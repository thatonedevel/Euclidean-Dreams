using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    // reference to actual camera component
    [SerializeField] private Camera levelCamera;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] GameObject rigHelper;

    [Header("Camera angle limits")]
    [SerializeField] private float maxX;
    [SerializeField] private float maxY;
    [SerializeField] private float maxZ;
    [SerializeField] private float rotationAmount = 45;

    // camera controls etc
    private InputAction cameraLookAction;

    // angle tracking
    [Header("Debug Info - Rotation information")]
    [SerializeField] private Axes lastAxisRotatedOn = Axes.NONE;
    [SerializeField] private Vector3 targetAngles = Vector3.zero;
    [SerializeField] private bool isRotating = false;
    [SerializeField] private float percentInc = 0;
    [SerializeField] private float currentPercent = 0;
    [SerializeField] private Vector3 lerpStartAngles = Vector3.zero;
    [SerializeField] private float elapsedLerpTime = 0;
    [SerializeField] private float neededLerpTime = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraLookAction = InputSystem.actions.FindAction("Look");

        // test lerp for negative angles
        Debug.Log("Test lerp on a negative target (12%): " + Vector3.Lerp(Vector3.zero, new Vector3(0, -45, 0), 0.12f));
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = cameraLookAction.ReadValue<Vector2>();
        Vector3 rotation = Vector3.zero;

        SetRotationVector(ref rotation, movement);

        // only allow camera rotation in 3D
        if (!levelCamera.orthographic)
            HandleCameraRotation(rotation);
    }

    private void SetRotationVector(ref Vector3 rotation, Vector2 movement)
    {
        // read in the following order - left, right, up, down
        if (movement.x > 0)
        {
            rotation.Set(0, -rotationAmount, 0);
        }
        else if (movement.x < 0)
        {
            rotation.Set(0, rotationAmount, 0);
        }
        else if (movement.y > 0)
        {
            rotation.Set(rotationAmount, 0, 0);
        }
        else if (movement.y < 0)
        {
            rotation.Set(-rotationAmount, 0, 0);
        }
    }

    /*private Vector3 FindNextAngle(Vector3 rotation)
    {
        // treat each axis as a cardinal direction
        // x = left / right
        // y = up / down
        switch (lastAxisRotatedOn)
        {
            case Axes.NONE:
                // default, so left / right is x axis & z is up / down
                return rotation;
            case Axes.X:
                //
        }
    }*/

    private void HandleCameraRotation(Vector3 rotationVal)
    {
        if (!isRotating) 
        {
            // set target angle
            targetAngles = transform.rotation.eulerAngles + rotationVal;
            currentPercent = 0;
            lerpStartAngles = transform.eulerAngles;

            // check if we need to lerp at all
            if (targetAngles == transform.eulerAngles)
            {
                return;
            }

            // calculate the needed time - author French (2020, updated 2024) indicated lerp should not be used with speed
            // speed = dist / time => time = dist / speed
            neededLerpTime = Vector3.Distance(lerpStartAngles, targetAngles) / rotationSpeed;
            // calculate the per frame %
            isRotating = true;
        }
        if (isRotating)
        {
            // (French, 2020 [Updated 2024])
            currentPercent = elapsedLerpTime / neededLerpTime;

            Vector3 newEuler = Vector3.Lerp(lerpStartAngles, targetAngles, currentPercent);
            elapsedLerpTime += Time.deltaTime;

            // set rotation
            transform.eulerAngles = newEuler;

            // set rig helper rotation but don't change its x axis
            rigHelper.transform.eulerAngles = new Vector3(rigHelper.transform.eulerAngles.x, newEuler.y, newEuler.z);

            // check if we need to snap the rotation
            if (currentPercent >= 1)
            {
                // stop rotation, reset values
                transform.eulerAngles = targetAngles;
                elapsedLerpTime = 0;
                neededLerpTime = 0;
                currentPercent = 0;
                isRotating = false;
            }
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