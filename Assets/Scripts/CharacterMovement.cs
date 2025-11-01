using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Data")]
    [SerializeField] private float movementSpeed = 5.0f;

    [Header("Object References")]
    [SerializeField] private Rigidbody characterRigidbody;
    [SerializeField] private GameObject levelCameraRoot;

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

        Vector3 moveVector = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed;

        Vector3 destination = levelCameraRoot.transform.TransformDirection(moveVector) + transform.position;

        // move rigidbody
        characterRigidbody.MovePosition(Vector3.Lerp(transform.position, destination, Time.deltaTime));

        transform.LookAt(new Vector3(destination.x, transform.position.y, destination.z));
    }
}
