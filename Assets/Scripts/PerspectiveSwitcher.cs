using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PerspectiveSwitcher : MonoBehaviour
{
    [SerializeField] private Camera levelCamera;

    [Header("Perspective Settings")]
    [SerializeField] private float fieldOfView = 60;

    [Header("Orthographic Settings")]
    [SerializeField] private float size = 5;

    [Header("Collision Settings")]
    [SerializeField] private float collisionThickness;

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

        HashSet<GameObject> detectedGeometry = new(); // use hashset as its more performant for searching

        Vector3 camRotationEuler = levelCamera.transform.parent.localEulerAngles;

        float horAngle = fieldOfView;
        float vertAngle = 360 - (fieldOfView * 2);

        // use screenpoints to determine the raycast direction etc.
        float screenPointXIncr = 1.0f / fieldOfView;
        float screenPointYIncr = 1.0f / vertAngle;

        RaycastHit hitData;

        for (float spY = 0; spY <= 1; spY += screenPointYIncr)
        {
            for (float spX = 0; spX <= 1; spX += screenPointXIncr)
            {
                Ray outRay = levelCamera.ScreenPointToRay(new Vector3(spX, spY));
                Physics.Raycast(ray:outRay, hitInfo: out hitData);

                if (hitData.collider != null)
                {
                    // we hit something, check it is level geometry
                    if (hitData.collider.CompareTag("LevelGeometry"))
                    {
                        // add it to the hash set
                        detectedGeometry.Add(hitData.collider.gameObject);
                    }
                }
            }
        }

        // at this point we have all the level geometry
        // next we need to determine the needed collision data
        // if we're looking down, generate it aroud the geometry
        if (levelCamera.transform.eulerAngles.y == 90)
            GenerateCollisionAroundGeo();
    }

    private void GenerateCollisionAroundGeo()
    {

    }
}
