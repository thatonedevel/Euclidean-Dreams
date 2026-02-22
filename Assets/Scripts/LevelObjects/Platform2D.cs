using System;
using GameConstants.Enumerations;
using UnityEngine;

namespace LevelObjects
{
    public class Platform2D : MonoBehaviour
    {
        [Header("Platform Settings")]
        [SerializeField] private Axes traversableAxis = Axes.Z;
        
        [Header("Object References")]
        [SerializeField] private Collider platformCollider;
        [SerializeField] private MeshRenderer platformRenderer;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // subscribe to perspective switch event
            PerspectiveSwitcher.OnDimensionsSwitched_Early += OnDimensionsSwitch_Handler;
            platformCollider.enabled = false;
        }

        private void OnDestroy()
        {
            // unsubscribe from the event
            PerspectiveSwitcher.OnDimensionsSwitched_Early -= OnDimensionsSwitch_Handler;
        }

        private void OnDimensionsSwitch_Handler(Dimensions newDim)
        {
            if (newDim == Dimensions.THIRD)
            {
                platformCollider.enabled = false;
                //platformRenderer.enabled = false;
            }
            else
            {
                // check it meets the specified axis
                if (PerspectiveSwitcher.CurrentObservedAxis == traversableAxis)
                {
                    // enable the collider
                    platformCollider.enabled = true;
                    //platformRenderer.enabled = true;
                }
                else
                {
                    platformCollider.enabled = false;
                }
            }
        }
    }
}
