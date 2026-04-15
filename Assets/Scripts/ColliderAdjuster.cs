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
                attachedColliders[i].center = attachedColliders[i].transform.InverseTransformPoint(
                    SetValueOnAxis(attachedColliders[i].center,
                        GameController.Singleton.GetPlayerAxisValue(axis), axis)
                );
                // HACK: do this to make sure that the button collider's y position isn't changed
                attachedColliders[i].center = SetValueOnAxis(attachedColliders[i].center, defaultSettings[i].position.y, Axes.Y);
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

    private void YAxisColliderAdjustment()
    {
        // adjust the position of the colliders to be just under the player - make sure heighest collider is directly under
        // means buttons will still work etc
        attachedColliders.Sort((a, b) => (int)(a.center.y - b.center.y));
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