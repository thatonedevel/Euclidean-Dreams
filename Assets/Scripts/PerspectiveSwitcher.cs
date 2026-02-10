using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GameConstants;
using GameConstants.Enumerations;
using Unity.VisualScripting;

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

    private const int CHECK_ANGLE = 90;
    private const int ANGLE_THRESHOLD = 5;

    // lambdas
    private bool IsLookingDownXAxis() => EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles) == new Vector3(0, 90, 0)
        || EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles) == new Vector3Int(0, -90, 0)
        || EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles) == new Vector3Int(0, 270, 0);

    private bool IsLookingDownYAxis() => EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles).x == 90 && (EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles).z % 90 == 0 || EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles).z == 360);

    private bool IsLookingDownZAxis() => EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles) == new Vector3Int(0, 180, 0)
        || EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles) == new Vector3Int(0, -180, 0)
        || EDreams.Util.ConvertVec3ToInt(levelCamera.transform.parent.eulerAngles) == new Vector3Int(0, 0, 0);

    public static Dimensions CurrentDimension { get; private set; } = Dimensions.THIRD;
    public static Axes CurrentObservedAxis { get; private set; } = Axes.Z;

    public static GameObject[] CurrentVisibleGeometryIn2D { get; private set; } = { };

    // event fired when switching dimensions
    public static event Action<Dimensions> OnDimensionsSwitched;

    // input
    private InputAction perspectiveSwitchAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        perspectiveSwitchAction = InputSystem.actions.FindAction(Constants.ACTION_SWITCH_PERSPECTIVE);
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
        Debug.Log("Camera rig euler rotation: " +  levelCamera.transform.parent.eulerAngles);
        Debug.Log("Camera rig local euler rotation: " + levelCamera.transform.parent.localEulerAngles);

        if (levelCamera.orthographic)
        {
            // switch to perspective projection
            SetPlayer3DPos();
            levelCamera.orthographic = false;
            levelCamera.fieldOfView = fieldOfView;
            
            CurrentDimension = Dimensions.THIRD;
            // clear detected geometry array
            CurrentVisibleGeometryIn2D = Array.Empty<GameObject>();
            // fire dimension switch event
            OnDimensionsSwitched?.Invoke(CurrentDimension);
        }
        else
        {
            // check we're aligned with one of the three axes
            if (IsLookingDownXAxis() || IsLookingDownYAxis() || IsLookingDownZAxis())
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
                    // check that the geometry isn't genned collision
                    if (!hitData.collider.CompareTag(Constants.TAG_GENERATED_COLLIDER))
                        detectedGeometry.Add(hitData.collider.gameObject);
                }

                screenX += xScale;
            }
            screenX = 0;
            screenY += yScale;
        }

        // at this point we have all the level geometry
        // next we need to determine the needed collision data
        // if we're looking down, generate it aroud the geometry
        if (IsLookingDownYAxis())
        {
            // looking down y axis
            Debug.Log("Camera was at appropriate angle to read as facing straight down");
            CurrentObservedAxis = Axes.Y;
        }
        else if (IsLookingDownXAxis())
        {
            // looking down the x axis
            Debug.Log("Looking down x axis");
            CurrentObservedAxis = Axes.X;
        }
        else if (IsLookingDownZAxis())
        {
            // looking down the z axis
            Debug.Log("Looking down z axis");
            CurrentObservedAxis = Axes.Z;
        }

        CalculatePlayerAxisPosition(detectedGeometry, CurrentObservedAxis);
        // set the current detected geometry. will be empty when in 3D
        CurrentVisibleGeometryIn2D = detectedGeometry.ToArray();
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
        float neededAxisLevel = GetClosestAxisPositionFromGeo(levelGeo, axis);

        Debug.Log("Highest geometry: " + geoArray[0]);
        Debug.Log("Calculated" + axis + " level: " + neededAxisLevel);

        // disable the gravity
        playerRigidbody.useGravity = false;
        SetPlayerAxisAsValue(neededAxisLevel, axis);
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

    private void SetPlayerAxisAsValue(Vector3 extractionVector, Axes axis)
    {
        // override that takes a vector3 and uses the value on the specified axis
        switch (axis) 
        {
            case Axes.X:
                transform.position = new Vector3(extractionVector.x, transform.position.y, transform.position.z);
                break;
            case Axes.Y:
                transform.position = new Vector3(transform.position.x, extractionVector.y, transform.position.z);
                break;
            case Axes.Z:
                transform.position = new Vector3(transform.position.x, transform.position.y, extractionVector.z);
                break;
        }
    }

    private float GetClosestAxisPositionFromGeo(HashSet<GameObject> geometry, Axes axis)
    {
        // called when looking straight down
        // get the highest y level & apply that to the player
        GameObject[] geoArray = geometry.ToArray();

        Debug.Log("Detected geo amount: " + geoArray.Length);

        float neededAxisValue = 0;

        // use a switch as we need to sort by a specified axis

        switch (axis)
        {
            case Axes.X:
                Array.Sort(geoArray, (GameObject a, GameObject b) => { return (int)(a.transform.position.x - b.transform.position.x) * -1; });
                neededAxisValue = geoArray[0].GetComponent<Collider>().bounds.max.x;
                break;
            case Axes.Y:
                Array.Sort(geoArray, (GameObject a, GameObject b) => { return (int)(a.transform.position.y - b.transform.position.y) * -1; });
                // first item will now be at the highest y level
                neededAxisValue = geoArray[0].GetComponent<Collider>().bounds.max.y;
                break;
            case Axes.Z:
                Array.Sort(geoArray, (GameObject a, GameObject b) => { return (int)(a.transform.position.z - b.transform.position.z); });
                neededAxisValue = geoArray[0].GetComponent<Collider>().bounds.max.z;
                break;
        }

        return neededAxisValue; // fail state
    }


    private void SetPlayer3DPos()
    {
        if (CurrentObservedAxis == Axes.Y)
        {
            // calculate the needed position of the player on each axis when returning to 3d
            // perform a raycast from the player going straight down
            Vector3 colliderCenterY = new(0, playerCollider.bounds.center.y, 0);

            Ray playerRay = new Ray(transform.position + colliderCenterY, Vector3.down);
            Debug.DrawRay(playerRay.origin, playerRay.direction * Constants.MAX_RAYCAST_DISTANCE, Color.red, 1);
            RaycastHit hit;

            Physics.Raycast(ray: playerRay, hitInfo: out hit, Constants.MAX_RAYCAST_DISTANCE, layerMask: raycastingMask.value);

            if (hit.collider != null)
            {
                // get the max y of the collider & set that as the player's new y
                transform.position = new Vector3(transform.position.x, 
                    hit.collider.bounds.max.y, transform.position.z);
            }
        }
        else
        {
            // we're looking through one of the horizontal axes (x/z)
            CalculatePlayerHorizontalPosition();
        }
        // reenable the gravity too doofus
        playerRigidbody.useGravity = true;
    }    

    private void CalculatePlayerHorizontalPosition()
    {
        // use this to calculate the horizontal axis value for the current axis when switching back to 3D

        // get the screen space position of the player, then go down by a couple of pixels
        Vector3 playerOffsetScreenSpacePos = levelCamera.WorldToScreenPoint(transform.position) - new Vector3(0, 5);

        // we then do a screen pos to world pos & raycast that - therefore we can get geometry under the player
        Ray groundRay = levelCamera.ScreenPointToRay(playerOffsetScreenSpacePos);
        RaycastHit currentHit;

        // regular raycast
        Debug.DrawRay(groundRay.origin, groundRay.direction * Constants.MAX_RAYCAST_DISTANCE, Color.green, 1);
        Physics.Raycast(ray: groundRay, hitInfo: out currentHit, maxDistance: Constants.MAX_RAYCAST_DISTANCE, layerMask: raycastingMask.value);

        // check for a hit & set position
        if (currentHit.collider != null) 
        {
            Vector3 centerHoriPos = currentHit.collider.bounds.center + new Vector3(0, currentHit.collider.bounds.extents.y);
            SetPlayerAxisAsValue(centerHoriPos, CurrentObservedAxis);
        }
    }
}