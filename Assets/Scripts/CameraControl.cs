using System.Collections.Generic;
using GameConstants;
using UnityEngine;
using UnityEngine.InputSystem;
using LevelObjects;

public class CameraControl : MonoBehaviour
{
    // reference to actual camera component
    [SerializeField] private Camera levelCamera;
    [SerializeField] GameObject rigHelper;

    [Header("Scales")]
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float cameraZoomScale = 5.0f;

    [Header("Camera angle limits")]
    [SerializeField] private float maxX;
    [SerializeField] private float maxY;
    [SerializeField] private float maxZ;
    [SerializeField] private float rotationAmount = 45;

    // camera controls etc
    private InputAction cameraLookAction;
    private InputAction cameraProfZoomAction; // IT WAS ME BARRY, I'M THE ONE WHO MADE YOUR UNIT TESTS FAIL
    
    // relative positions for automatic camera adjustment & player reference
    private Vector3 relativeDirectionToPlayer = Vector3.zero;
    private GameObject playerCharacter;
    private Dictionary<int, CameraTransformSnapshot> transformSnapshots = new();

    // angle tracking
    [Header("Debug Info - Rotation information")]
    [SerializeField] private Vector3 targetAngles;
    [SerializeField] private bool isRotating;
    [SerializeField] private float percentInc;
    [SerializeField] private float currentPercent;
    [SerializeField] private Vector3 lerpStartAngles;
    [SerializeField] private float elapsedLerpTime;
    [SerializeField] private float neededLerpTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraLookAction = InputSystem.actions.FindAction(Constants.ACTION_ROTATE_CAMERA);
        cameraProfZoomAction = InputSystem.actions.FindAction(Constants.ACTION_ZOOM_CAMERA);

        // test lerp for negative angles
        //Debug.Log("Test lerp on a negative target (12%): " + Vector3.Lerp(Vector3.zero, new Vector3(0, -45, 0), 0.12f));

        if (playerCharacter == null)
            playerCharacter = GameObject.FindWithTag(Constants.TAG_PLAYER);
        
        relativeDirectionToPlayer = playerCharacter.transform.position - transform.parent.position;
        Portal.OnPlayerLeftPortal += PortalExitHandler;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = cameraLookAction.ReadValue<Vector2>();
        Vector3 rotation = Vector3.zero;
        Vector2 zoomAmount = cameraProfZoomAction.ReadValue<Vector2>();


        SetRotationVector(ref rotation, movement);

        // only allow camera rotation in 3D
        if (!levelCamera.orthographic)
            HandleCameraRotation(rotation);

        ZoomCamera(zoomAmount);
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
        
        // check the rotation vector's x value - if it's not 0 and it would put
        // the player over the limit, cancel it
        if (rotation.x != 0)
        {
            print("target rotation:" + (Mathf.Round(transform.eulerAngles.x) + rotation.x));
            if (transform.eulerAngles.x < 180)
                rotation.x = Mathf.Abs(Mathf.Round(transform.eulerAngles.x) + rotation.x) > 90? 0 : rotation.x;
            else
                rotation.x = Mathf.Abs(Mathf.Round(transform.eulerAngles.x) + rotation.x) >= 270? rotation.x : 0;
        }
    }

    private void ZoomCamera(Vector2 input)
    {
        // adjusts the camera's zoom level based on the y axis of the input vector
        float delta = input.y * cameraZoomScale * Time.deltaTime;

        if (!levelCamera.orthographic)
        {
            // 3d, adjust camera pos
            levelCamera.transform.position += levelCamera.transform.forward * delta;
        }
        else
        {
            // adjust the camera size
            levelCamera.orthographicSize += delta * -1;
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
                transform.eulerAngles = new Vector3(Mathf.Round(transform.eulerAngles.x), Mathf.Round(transform.eulerAngles.y), 0);
                elapsedLerpTime = 0;
                neededLerpTime = 0;
                currentPercent = 0;
                isRotating = false;
            }
        }
    }

    private void PortalExitHandler(Portal exitPortal, bool shouldAdjustCamera)
    {
        var linkedTransform = exitPortal.GetLinkedPortalTransform();
        if (shouldAdjustCamera)
        {
            // check if an entry for the portal we're coming out of exists
            if (transformSnapshots.ContainsKey(exitPortal.gameObject.GetInstanceID()))
            {
                // restore to the settings stored here
                var snap = transformSnapshots[exitPortal.gameObject.GetInstanceID()];
                Vector3 relPortalPos = snap.relativeCamPosition;
                Vector3 parentForward = snap.parentForwardDir;
                Vector3 parentUp = snap.parentUpDir;
                Vector3 parentRight = snap.parentRightDir;
                
                transform.parent.position = relPortalPos + exitPortal.transform.position;
                
                transform.parent.forward = parentForward;
                transform.parent.up = parentUp;
                transform.parent.right = parentRight;
            }
            else
            {
                MakeEntryForPortal(exitPortal);
            }
        }
    }

    private void MakeEntryForPortal(Portal exitPortal)
    {
        var linkedTransform = exitPortal.GetLinkedPortalTransform();
        
        // calculate new transform and create an entry for the current transform settings
        // calculate relative position to entry portal & rotation
        Vector3 relPosToPortal = exitPortal.GetLinkedPortalPosition() - transform.parent.position;
        Vector3 relEulerRot = transform.parent.eulerAngles - exitPortal.GetLinkedPortalEulerAngles();
        // adjust the height of the camera so that the player can see this section of the level more easily
        Vector3 newPosition = exitPortal.transform.InverseTransformDirection(relPosToPortal);
                
        Vector3 newForward = transform.parent.forward;
        Vector3 newUp = transform.parent.up;
        Vector3 newRight = transform.parent.right;
            
        // make sure our up matches the exit portal's up
        transform.parent.up = exitPortal.transform.up;
            
        // next check if either the forward or right directions of the entry portal matched
        // if they did, apply this to the camera
        if (transform.forward == linkedTransform.forward)
            newForward = exitPortal.transform.forward;
        if (transform.right == linkedTransform.right)
            newRight = exitPortal.transform.right;
                
        // create the snapshot
        var snapshot = new CameraTransformSnapshot(relPosToPortal, transform.parent.forward, transform.parent.up, transform.parent.right);
                
        transformSnapshots.Add(exitPortal.GetLinkedPortalID(), snapshot);
                
        transform.parent.position = newPosition;
                
        transform.parent.forward = newForward;
        transform.parent.up = newUp;
        transform.parent.right = newRight;
                
        //transform.parent.eulerAngles = newRotation;
    }
}

public struct CameraTransformSnapshot
{
    // struct used for holding snapshots of the camera's transform state
    public Vector3 relativeCamPosition;
    public Vector3 parentForwardDir;
    public Vector3 parentUpDir;
    public Vector3 parentRightDir;

    public CameraTransformSnapshot(Vector3 relativeCamPosition, Vector3 parentForwardDir, Vector3 parentUpDir, Vector3 parentRightDir)
    {
        this.relativeCamPosition = relativeCamPosition;
        this.parentForwardDir = parentForwardDir;
        this.parentUpDir = parentUpDir;
        this.parentRightDir = parentRightDir;
    }
}