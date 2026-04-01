using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using GameConstants.Enumerations;
using Managers;
using UnityEngine.UIElements;

namespace LevelObjects.Switches
{
    public abstract class ASwitch : MonoBehaviour
    {
        [Header("Common Settings")]
        [SerializeField] public UnityEvent onSwitchEnabled;
        [SerializeField] protected string[] validTags;

        private List<BoxCollider> attachedColliders = new();
        private List<ColliderSettings> defaultSettings = new();

        // event listener for the dimension switch to adjust colliders
        protected virtual void Start()
        {
            BoxCollider[] parent = GetComponents<BoxCollider>();
            BoxCollider[] childrens = GetComponentsInChildren<BoxCollider>();
            
            attachedColliders.AddRange(parent);
            attachedColliders.AddRange(childrens);
            
            // set up the settings structs
            for (int i = 0; i < attachedColliders.Count; i++)
            {
                defaultSettings.Add(new ColliderSettings(attachedColliders[i]));
            }
            
            // subscribe to dim switch event
            PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
        }

        protected virtual void OnDestroy()
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
                        attachedColliders[i].center = SetValueOnAxis(attachedColliders[i].center,
                            GameController.Singleton.GetPlayerAxisValue(axis), axis);
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
}

public struct ColliderSettings
{
    // struct to hold collider settings
    public Vector3 position;
    public Vector3 size;
    public bool isTrigger;

    public ColliderSettings(Collider target)
    {
        // copies specified info from target collider
        position = target.bounds.center;
        size = target.bounds.size;
        isTrigger = target.isTrigger;
    }
    
    public void CopySettingsToCollider(Collider target)
    {
        var targetB = target as BoxCollider;

        if (targetB != null)
        {
            targetB.center = position;
            targetB.size = size;
            targetB.isTrigger = isTrigger;
        }
    }
}
