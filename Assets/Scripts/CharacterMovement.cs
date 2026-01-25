using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private MovementAxis movableAxes = MovementAxis.XZ;

    [Header("Object References")]
    [SerializeField] private Rigidbody characterRigidbody;
    [SerializeField] private GameObject cameraRigDownAnchor;

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