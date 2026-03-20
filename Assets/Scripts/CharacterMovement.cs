using UnityEngine;
using UnityEngine.InputSystem;
using GameConstants.Enumerations;
using GameConstants;
using LevelObjects.ForceManipulators;
using LevelObjects;
using UnityEngine.ProBuilder;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private MovementAxisCombos movableAxes = MovementAxisCombos.XZ;
    [SerializeField] private LayerMask groundingMask;

    [Header("Object References")]
    [SerializeField] private Rigidbody characterRigidbody;
    [SerializeField] private Collider characterCollider;
    [SerializeField] private GameObject cameraRigDownAnchor;
    [SerializeField] private GameObject cameraParent;
    [SerializeField] private ConstantForce customGravityForce;

    // input actions
    private InputAction movementAction;
    private InputAction jumpAction;

    private bool isFirstUpdate = true;

    private Vector3 destination = Vector3.zero;
    private Vector3 dir =  Vector3.zero;
    private bool isMovingFromPortal = false;
    private Portal exitPortal = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementAction = InputSystem.actions.FindAction(Constants.ACTION_MOVE);
        jumpAction = InputSystem.actions.FindAction(Constants.ACTION_JUMP);

        PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
        AGravityManipulator.OnGravityChanged += GravityChangedHandler;
        Portal.OnPlayerLeftPortal += PortalExitHandler;
    }

    // Update is called once per frame
    void Update()
    {
        // get move vector
        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        Vector3 moveVector = Vector3.zero;
        Vector3 finalDirection = Vector3.zero;
        
        
        switch (movableAxes)
        {
            case MovementAxisCombos.XZ:
                moveVector = new Vector3(moveInput.x, 0, moveInput.y);
                break;
            case MovementAxisCombos.XY:
                moveVector = new Vector3(moveInput.x, 0, 0);
                break;
            case MovementAxisCombos.YZ:
                moveVector = new Vector3(moveInput.x, 0, 0);
                break;
        }

        if (!isMovingFromPortal)
            dir = cameraRigDownAnchor.transform.TransformDirection(moveVector);
        else
        {
            // check if we need to reset the flag
            
            
            Debug.Log("handling from portal");
            
            // calculate the target direction
            var transDir = cameraRigDownAnchor.transform.InverseTransformDirection(moveVector);
            Vector3 targetDir = exitPortal.transform.InverseTransformDirection(transDir);
            dir = Vector3.RotateTowards(transDir, targetDir * transDir.magnitude,
                Mathf.Rad2Deg * 360, 0.1f);
        }
    }

    private void FixedUpdate()
    {
        // handle all rigidbody movement here - as pointed out by ep1s0de (2021)
        // all programatic use of a rigidbody should be handled in FixedUpdate

        // fix to prevent character being moved on scene start
        if (isFirstUpdate)
        {
            isFirstUpdate = false;
            return;
        }
        
        // draw a line from the player character's current position to their destination position
        Debug.DrawLine(transform.position, transform.position + dir, Color.red, 5);

        //characterRigidbody.MovePosition(destination);
        characterRigidbody.MovePosition(transform.position + dir * movementSpeed * Time.fixedDeltaTime);
        
        transform.LookAt(transform.position + dir);

        if (PerspectiveSwitcher.CurrentDimension == Dimensions.SECOND)
        {
            // check the angle of the camera
            if (cameraParent.transform.localEulerAngles.x == 90)
            {
                HandleFallingWhenTopDown();
            }
        }
    }


    private void HandleFallingWhenTopDown()
    {
        // method to run grounding checks when we're in 2D and looking straight down
        Vector3 centeredOffset = new Vector3(0, characterCollider.bounds.center.y, 0);

        Ray groundCheckRay = new Ray(transform.position + centeredOffset, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(groundCheckRay.origin, groundCheckRay.direction * Constants.MAX_RAYCAST_DISTANCE, Color.yellow, 1);
        Physics.Raycast(ray: groundCheckRay, hitInfo: out hit, Constants.MAX_RAYCAST_DISTANCE, layerMask: groundingMask.value);

        if (hit.collider == null)
        {
            // we're not grounded
            characterRigidbody.useGravity = true;
        }
    }

    private void DimensionSwitchHandler(Dimensions newDimension)
    {
        // update the movable axes
        if (newDimension == Dimensions.THIRD)
        {
            // set to default
            movableAxes = MovementAxisCombos.XZ;
        }
        else
        {
            // check the observed angle
            switch (PerspectiveSwitcher.CurrentObservedAxis)
            {
                case Axes.Y:
                    movableAxes = MovementAxisCombos.XZ;
                    break;
                case Axes.X:
                    movableAxes = MovementAxisCombos.YZ;
                    break;
                case Axes.Z:
                    movableAxes = MovementAxisCombos.XY;
                    break;
            }
        }
    }

    private void GravityChangedHandler(Vector3 mavity)
    {
        if (mavity == Physics.gravity)
        {
            DisableCustomGravity();
        }
        else
        {
            Debug.Log("Setting new gravity: " + mavity * Physics.gravity.magnitude);
            SetCustomGravity(mavity);
        }

        if (mavity.y != 0)
            movableAxes = MovementAxisCombos.XZ; // normal gravity
        else if (mavity.x != 0)
            movableAxes = MovementAxisCombos.YZ;
        else if (mavity.z != 0)
            movableAxes = MovementAxisCombos.XY;
        
    }

    public void SetCustomGravity(Vector3 newGravityAcceleration)
    {
        // set custom gravity using a constant force component (Ryiah, 2015)
        customGravityForce.force = newGravityAcceleration;
        characterRigidbody.useGravity = false;
        customGravityForce.enabled = true;
        
        // also when called, make sure to stop the current movement of the character on this frame & adjust up direction
        destination = transform.position;
        // gravity pulls down so use the reverse direction as the new up
        transform.up = newGravityAcceleration.normalized * -1;
    }

    public void DisableCustomGravity()
    {
        customGravityForce.enabled = false;
        characterRigidbody.useGravity = true;
        transform.up = Physics.gravity.normalized * -1;
    }

    private void PortalExitHandler(Portal exit)
    {
        isMovingFromPortal = true;
        exitPortal =  exit;
    }
}