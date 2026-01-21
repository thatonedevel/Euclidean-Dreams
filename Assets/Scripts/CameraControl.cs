using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    // reference to actual camera component
    [SerializeField] private Camera levelCamera;
    [SerializeField] private float rotationSpeed = 5.0f;

    [Header("Camera angle limits")]
    [SerializeField] private float maxX;
    [SerializeField] private float maxY;
    [SerializeField] private float maxZ;

    // camera controls etc
    private InputAction cameraLookAction;

    // angle tracking
    private Axes lastAxisRotatedOn = Axes.NONE;
    private Vector3 targetAngles = Vector3.zero;
    private bool isRotating = false;
    private float percentInc = 0;
    private float currentPercent = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraLookAction = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = cameraLookAction.ReadValue<Vector2>();
        Vector3 rotation = Vector3.zero;

        SetRotationVector(ref rotation, movement);


        HandleCameraRotation(rotation);
    }

    private void SetRotationVector(ref Vector3 rotation, Vector2 movement)
    {
        // read in the following order - left, right, up, down
        if (movement.x > 0)
        {
            rotation.Set(0, -45, 0);
        }
        else if (movement.x < 0)
        {
            rotation.Set(0, 45, 0);
        }
    }

    //private Vector3 FindNextAngle(Vector3 rotation)
    //{
    //    // treat each axis as a cardinal direction
    //    // x = left / right
    //    // y = up / down
    //    switch (lastAxisRotatedOn)
    //    {
    //        case Axes.NONE:
    //            // default, so left / right is x axis & z is up / down
    //            return rotation;
    //        case Axes.X:
    //            //
    //    }
    //}

    private void HandleCameraRotation(Vector3 rotationVal)
    {
        if (!isRotating) 
        {
            // set target angle
            targetAngles = transform.rotation.eulerAngles;
            targetAngles += rotationVal;
            percentInc = rotationSpeed / (transform.eulerAngles - targetAngles).magnitude;
            currentPercent = 0;
            isRotating = true;
        }

        currentPercent += percentInc * Time.deltaTime;

        // lerp towards target position
        // t = % between a and b
        Vector3 newEuler = Vector3.Lerp(transform.eulerAngles, targetAngles, currentPercent);

        // set rotation & check if we're very close
        transform.eulerAngles = newEuler;

        if (Vector3.Distance(newEuler, targetAngles) <= rotationSpeed)
        {
            transform.eulerAngles = targetAngles;
            isRotating = false;
        }
    }
}

public enum Axes
{
    X,
    Y, 
    Z,
    NONE
}