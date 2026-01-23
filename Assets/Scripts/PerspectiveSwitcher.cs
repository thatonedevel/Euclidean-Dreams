using UnityEngine;
using UnityEngine.InputSystem;

public class PerspectiveSwitcher : MonoBehaviour
{
    [SerializeField] private Camera levelCamera;

    [Header("Perspective Settings")]
    [SerializeField] private float fieldOfView = 60;

    [Header("Orthographic Settings")]
    [SerializeField] private float size = 5;

    // input
    private InputAction perspectiveSwitchAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        perspectiveSwitchAction = InputSystem.actions.FindAction("Toggle Perspective");
    }

    // Update is called once per frame
    void Update()
    {
        if (perspectiveSwitchAction.WasPressedThisFrame())
        {
            SwitchCamPerspective();
        }
    }

    public void SetCamera(Camera newCam)
    {
        levelCamera = newCam;
    }

    private void SwitchCamPerspective()
    {
        if (levelCamera.orthographic)
        {
            // switch to perspective projection
            levelCamera.orthographic = false;
            levelCamera.fieldOfView = fieldOfView;
        }
        else
        {
            // switch to ortho projection
            levelCamera.orthographic = true;
            levelCamera.orthographicSize = size;
        }
    }

    private void GeoSortingRaycasts()
    {
        // use raycasts to determine the x / y / z pos we need to move the  player to
        Vector3 camRotationEuler = levelCamera.transform.parent.localEulerAngles;

        if (camRotationEuler.x == 90)
        {
            // we're looking straight down
        }
        else
        {
            // not looking straight down
        }
    }
}
