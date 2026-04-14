using System;
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
    private Portal lastExitPortal = null;
    
    // used to preseverve rotation on movement
    Vector3 forwardDirectionForRotaton = Vector3.forward;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementAction = InputSystem.actions.FindAction(Constants.ACTION_MOVE);
        jumpAction = InputSystem.actions.FindAction(Constants.ACTION_JUMP);

        PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
        GetComponent<CustomGravity>().OnGravityDirectionChanged += GravityChangedHandler;
        GetComponent<CustomGravity>().OnGravityRestored +=  GravityRestoredHandler;
        Portal.OnPlayerLeftPortal += PortalExitHandler;
    }

    private void OnDestroy()
    {
        // unsubscribe from all events
        PerspectiveSwitcher.OnDimensionsSwitched -= DimensionSwitchHandler;
        GetComponent<CustomGravity>().OnGravityDirectionChanged -= GravityChangedHandler;
        GetComponent<CustomGravity>().OnGravityDirectionChanged -= GravityChangedHandler;
        Portal.OnPlayerLeftPortal -= PortalExitHandler;
    }

    // Update is called once per frame
    void Update()
    {
        // get move vector
        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        Vector3 moveVector = Vector3.zero;
        
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
            if (moveVector.magnitude != 0)
            {
                Debug.Log("handling from portal");
                
                // calculate the target direction
                var transDir = cameraRigDownAnchor.transform.InverseTransformDirection(moveVector);
                Vector3 targetDir = lastExitPortal.transform.InverseTransformDirection(transDir);
    
                if (lastExitPortal.DoesRotationMatchLinkedPortal() || (lastExitPortal.IsPortalNotUpright() && lastExitPortal.ArePortalsParrallel()))
                {
                    dir = Vector3.Reflect(transDir, lastExitPortal.transform.forward);
                    Debug.DrawLine(transform.position, transform.position + (dir * 5), Color.red, 5);
                }
                else
                {
                    dir = Vector3.RotateTowards(transDir, targetDir * transDir.magnitude,
                                    Mathf.Rad2Deg * 360, 0.1f);
                }                
            }
            else
            {
                isMovingFromPortal = false;
            }
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
        
        transform.rotation = dir.magnitude != 0? Quaternion.LookRotation(dir, transform.up) : transform.rotation;
        
        //transform.LookAt(transform.position + dir); don't use this as it overrides rotation (StarManta, 2021)

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
        // used to adjust how the character should move when custom gravoty is applied
        
        // work out movement axis combo
        if (mavity.normalized == Vector3.up)
            movableAxes = MovementAxisCombos.XZ;
        else if (mavity.normalized == Vector3.right || mavity.normalized == Vector3.left)
            movableAxes = MovementAxisCombos.YZ;
        else if (mavity.normalized == Vector3.forward || mavity.normalized == Vector3.back)
            movableAxes = MovementAxisCombos.XY;
    }

    private void GravityRestoredHandler()
    {
        movableAxes = MovementAxisCombos.XZ;
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

    private void PortalExitHandler(Portal exit, bool t)
    {
        isMovingFromPortal = true;
        lastExitPortal =  exit;
    }
}