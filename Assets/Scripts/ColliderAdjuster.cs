using UnityEngine;
using GameConstants.Enumerations;
using GameConstants;
using System.Collections.Generic;
using Managers;


public class ColliderAdjuster : MonoBehaviour
{
    private List<BoxCollider> attachedColliders= new();
    private List<ColliderSettings> defaultSettings = new();
    
    private void Start()
    {
       // set up the lists
       attachedColliders.AddRange(GetComponents<BoxCollider>());
       // set the default collider settings
       
       foreach (var col in attachedColliders)
       {
           defaultSettings.Add(new ColliderSettings(col));
       }
       
       // set up the event sub for dim switching
       PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
    }

    private void OnDestroy()
    {
        PerspectiveSwitcher.OnDimensionsSwitched -= DimensionSwitchHandler;
    }
    
    protected virtual void DimensionSwitchHandler(Dimensions newDim)
    {
        // grab a reference to the player
        GameObject playerObject = GameObject.FindWithTag(Constants.TAG_PLAYER);
        
        if (newDim == Dimensions.THIRD)
        {
            // restore colliders to their default settings
            for (int i = 0; i < attachedColliders.Count; i++)
            {
                defaultSettings[i].CopySettingsToCollider(attachedColliders[i]);
            }
        }
        else
        {
            // change the collider settings so that they are centred on the player on the current axis
            for (int i = 0; i < attachedColliders.Count; i++)
            {
                // if it's a box collider set the center bounds
                Axes axis = PerspectiveSwitcher.CurrentObservedAxis;
                Vector3 colCenterGlobal = new();
                if (axis == Axes.X)
                {
                    // set collider center x to consider the player's x position
                    colCenterGlobal = attachedColliders[i].bounds.center;
                    colCenterGlobal = new Vector3(playerObject.transform.position.x, colCenterGlobal.y, colCenterGlobal.z);
                    
                    // transform back to local space
                    attachedColliders[i].center = transform.InverseTransformPoint(colCenterGlobal);
                }
                else if (axis == Axes.Z)
                {
                    // set collider center x to consider the player's x position
                    colCenterGlobal =  attachedColliders[i].bounds.center;
                    colCenterGlobal = new Vector3(colCenterGlobal.x, colCenterGlobal.y, playerObject.transform.position.z);
                    
                    // transform back to local space
                    attachedColliders[i].center = transform.InverseTransformPoint(colCenterGlobal);
                }
            }
        }
    }

    private Vector3 SetValueOnAxis(Vector3 inp, float newValue, Axes axis)
    {
        Vector3 vec = inp;
            
        switch (axis)
        {
            case Axes.X:
                vec.x = newValue;
                break;
            case Axes.Y:
                vec.y = newValue;
                break;
            case Axes.Z:
                vec.z = newValue;
                break;
        }

        return vec;
    }
}

public struct ColliderSettings
{
    // struct to hold collider settings
    public Vector3 position;
    public Vector3 size;
    public bool isTrigger;

    public ColliderSettings(BoxCollider target)
    {
        // copies specified info from target collider
        position = target.center;
        size = target.size;
        isTrigger = target.isTrigger;
    }
    
    public void CopySettingsToCollider(BoxCollider target)
    {
        target.center = position;
        target.size = size;
        target.isTrigger = isTrigger;
    }
}