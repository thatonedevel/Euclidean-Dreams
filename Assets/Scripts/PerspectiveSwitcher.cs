using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GameConstants.Enumerations;

public class PerspectiveSwitcher : MonoBehaviour
{
    [Header("Object / Component References")]
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private Camera levelCamera;
    [SerializeField] private Collider playerCollider;

    [Header("Perspective Settings")]
    [SerializeField] private float fieldOfView = 60;

    [Header("Orthographic Settings")]
    [SerializeField] private float size = 5;

    [Header("Collision Settings")]
    [SerializeField] private float collisionThickness;
    [SerializeField] private LayerMask raycastingMask;

    public static Dimensions CurrentDimension { get; private set; } = Dimensions.THIRD;

    // event fired when switching dimensions
    public static event Action<Dimensions> OnDimensionsSwitched;

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
            SetPlayer3DPos();
            CurrentDimension = Dimensions.THIRD;
        }
        else
        {
            // switch to ortho projection
            levelCamera.orthographic = true;
            levelCamera.orthographicSize = size;
            Debug.Log("Running the raycasts");
            GeoSortingRaycasts();
            CurrentDimension = Dimensions.SECOND;
        }

        // fire dimension switch event
        OnDimensionsSwitched?.Invoke(CurrentDimension);
    }

    private void GeoSortingRaycasts()
    {
        // use raycasts to determine the x / y / z pos we need to move the  player to

        HashSet<GameObject> detectedGeometry = new(); // use hashset as its more performant for searching

        Vector3 camRotationEuler = levelCamera.transform.parent.localEulerAngles;

        float horAngle = fieldOfView;
        float vertAngle = 360 - (fieldOfView * 4);

        float screenX = 0;
        float screenY = 0;

        float xScale = Screen.width / horAngle;
        float yScale = Screen.height / vertAngle;

        print("x scale: " + xScale);
        print("y scale: " + yScale);

        print("hor angle: " + horAngle);
        print("vert angle: " + vertAngle);

        float rayLen = (levelCamera.farClipPlane - levelCamera.nearClipPlane) / 2.0f;
        
        RaycastHit hitData;
        // use screenpoints to determine the raycast direction etc.

        //print("X incrementer for loop: " + screenPointXIncr);
        //print("Y incrementer for loop: " + screenPointYIncr);

        // use the actual angles for looping, not the calculated floats
        for (int i = 0; i <= vertAngle; i++) // inclusive loop as we want the whole screen
        {
            for (int j = 0; j <= horAngle; j++)
            {
                Ray outRay = levelCamera.ScreenPointToRay(new Vector3(screenX, screenY, rayLen));

                // draw the ray
                Debug.DrawRay(outRay.origin, outRay.direction * rayLen, Color.white, 1);
                Physics.Raycast(ray: outRay, hitInfo: out hitData, maxDistance: rayLen, layerMask: raycastingMask.value);

                // if the ray hit level geometry, add it to the hash set
                if (hitData.collider != null)
                {
                    detectedGeometry.Add(hitData.collider.gameObject);
                }

                screenX += xScale;
            }
            screenX = 0;
            screenY += yScale;
        }

        //// at this point we have all the level geometry
        //// next we need to determine the needed collision data
        //// if we're looking down, generate it aroud the geometry
        if (levelCamera.transform.parent.eulerAngles.x == 90)
        {
            Debug.Log("Camera was at appropriate angle to read as facing straight down");
            CalculatePlayerAxisPosition(detectedGeometry, Axes.Y);
        }
        else if (levelCamera.transform.parent.eulerAngles.x != 90 && levelCamera.transform.parent.eulerAngles.x % 90 == 0)
        {
            // we're looking straight on
            // calculate the character's depth
        }

    }

    private void CalculatePlayerAxisPosition(HashSet<GameObject> levelGeo, Axes axis)
    {
        // called when looking straight down
        // get the highest y level & apply that to the player
        GameObject[] geoArray = levelGeo.ToArray();

        Debug.Log("Detected geo amount: " + geoArray.Length);

        // remember neg -> a before b, positive -> b before a
        Array.Sort(geoArray, (GameObject a, GameObject b) => { return (int)(a.transform.position.y - b.transform.position.y) * -1; });

        // first item will now be at the highest y level
        float neededYLevel = geoArray[0].GetComponent<Collider>().bounds.max.y;

        Debug.Log("Highest geometry: " + geoArray[0]);
        Debug.Log("Calculated y level: " + neededYLevel);

        // disable the gravity
        playerRigidbody.useGravity = false;
        SetPlayerAxisAsValue(neededYLevel, axis);
    }

    private void SetPlayerAxisAsValue(float newValue, Axes axis)
    {
        // sets the specified axis of the player's transform to the supplied value
        switch (axis)
        {
            case Axes.X:
                transform.position = new Vector3(newValue, transform.position.y, transform.position.z);
                break;
            case Axes.Y:
                transform.position = new Vector3(transform.position.x, newValue, transform.position.z);
                break;
            case Axes.Z:
                transform.position = new Vector3(transform.position.x, transform.position.y, newValue);
                break;
        }
    }

    private float GetClosestAxisPositionFromGeo(HashSet<GameObject> geometry, Axes axis)
    {
        // called when looking straight down
        // get the highest y level & apply that to the player
        GameObject[] geoArray = geometry.ToArray();

        Debug.Log("Detected geo amount: " + geoArray.Length);

        // use a switch as we need to sort by a specified axis
        Array.Sort(geoArray, (GameObject a, GameObject b) => { return (int)(a.transform.position.y - b.transform.position.y) * -1; });

        // first item will now be at the highest y level
        float neededYLevel = geoArray[0].GetComponent<Collider>().bounds.max.y;

        return 1; // fail state
    }


    private void SetPlayer3DPos()
    {
        // calculate the needed position of the player on each axis when returning to 3d
        // perform a raycast from the player going straight down
        Vector3 colliderCenterY = new(0, playerCollider.bounds.center.y, 0);


        Ray playerRay = new Ray(transform.position + colliderCenterY, Vector3.down);
        Debug.DrawRay(playerRay.origin, playerRay.direction * 100, Color.red, 1);
        RaycastHit hit;

        Physics.Raycast(ray: playerRay, hitInfo: out hit, 100, layerMask: raycastingMask.value);

        if (hit.collider != null)
        {
            // get the max y of the collider & set that as the player's new y
            transform.position = new Vector3(transform.position.x, 
                hit.collider.bounds.max.y, transform.position.z);
            // reenable the gravity too doofus
            playerRigidbody.useGravity = true;
        }
    }    
}