using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    // reference to actual camera component
    [SerializeField] private Camera levelCamera;
    [SerializeField] private float rotationSpeed = 5.0f;

    // camera movement action
    InputAction cameraRotateAction;
    InputAction cameraRotateButton;
    InputAction cameraZoomAction;
    InputAction perspectiveToggleAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // assign actions
        cameraRotateAction = InputSystem.actions.FindAction("Look");
        cameraRotateButton = InputSystem.actions.FindAction("Look Press");
        cameraZoomAction = InputSystem.actions.FindAction("Zoom");
        perspectiveToggleAction = InputSystem.actions.FindAction("TogglePerspective");
    }

    // Update is called once per frame
    void Update()
    {
        // write controls around mouse input for now
        
        // camera rotation
        Vector2 camRotation = cameraRotateAction.ReadValue<Vector2>();

        if (cameraRotateButton.IsPressed())
        {
            transform.Rotate(new Vector3(camRotation.y * -1, camRotation.x, 0)
                * Time.deltaTime * rotationSpeed, Space.World);
        }
    }
}
