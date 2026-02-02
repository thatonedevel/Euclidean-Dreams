using UnityEngine;
using UnityEngine.InputSystem;
using GameConstants.Enumerations;
using GameConstants;

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

    // input actions
    InputAction movementAction;
    InputAction jumpAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movementAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");

        PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
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
                moveVector = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed * Time.deltaTime;
                break;
            case MovementAxisCombos.XY:
                moveVector = new Vector3(moveInput.x, 0, 0) * movementSpeed * Time.deltaTime;
                break;
            case MovementAxisCombos.YZ:
                moveVector = new Vector3(moveInput.x, 0, 0) * movementSpeed * Time.deltaTime;
                break;
        }
        
        Vector3 destination = cameraRigDownAnchor.transform.TransformDirection(moveVector) + transform.position;

        // move rigidbody
        characterRigidbody.MovePosition(destination);

        transform.LookAt(new Vector3(destination.x, transform.position.y, destination.z));

        // if we're in 2D, perform the grounding check
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
}