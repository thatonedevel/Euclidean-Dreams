using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private MovementAxis movableAxes = MovementAxis.XZ;
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
    }

    // Update is called once per frame
    void Update()
    {
        // get move vector
        Vector2 moveInput = movementAction.ReadValue<Vector2>();
        Vector3 moveVector = Vector3.zero;

        switch (movableAxes)
        {
            case MovementAxis.XZ:
                moveVector = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed * Time.deltaTime;
                break;
            case MovementAxis.XY:
                moveVector = new Vector3(moveInput.x, moveInput.y, 0) * movementSpeed * Time.deltaTime;
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

        Ray groundCheckRay = new Ray(transform.position + centeredOffset, Vector3.down); // using transform down for future proofing
        RaycastHit hit;

        Debug.DrawRay(groundCheckRay.origin, groundCheckRay.direction * 100, Color.yellow, 1);
        Physics.Raycast(ray: groundCheckRay, hitInfo: out hit, 100, layerMask: groundingMask.value);

        if (hit.collider == null)
        {
            // we're not grounded
            characterRigidbody.useGravity = true;
        }
    }

    private void DimensionSwitchHandler(Dimensions newDimension)
    {
        
    }
}

public enum MovementAxis
{
    XZ,
    XY
}