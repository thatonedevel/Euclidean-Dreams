using System;
using UnityEngine;
using GameConstants.Enumerations;
using GameConstants;
using LevelObjects.ForceManipulators;

namespace Animation
{
    public class CustomBillboarder : MonoBehaviour
    {
        // billboards the sprite to face towards the camera, & enables/disables the sprite or mesh when in a given perspective
        [Header("Object References")] 
        [SerializeField] private GameObject oobiSprite;
        [SerializeField] private GameObject oobiModel;
        [SerializeField] private CustomGravity customGravity; 

        private Camera camera;
        private bool hasGravity = false;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
            camera = GameObject.FindGameObjectWithTag(Constants.TAG_CAMERA_MAIN).GetComponent<Camera>();
            
            if (customGravity == null)
                hasGravity = transform.parent.gameObject.TryGetComponent<CustomGravity>(out customGravity);
            else
            {
                hasGravity = true;
            }
        }

        private void OnDestroy()
        {
            PerspectiveSwitcher.OnDimensionsSwitched -= DimensionSwitchHandler;
        }

        private void DimensionSwitchHandler(Dimensions newDim)
        {
            if (newDim == Dimensions.SECOND)
            {
                // 2d
                // enable the sprite & set the global rotation to match the camera rotation
                oobiModel.SetActive(false);
                oobiSprite.SetActive(true);
                var pos = CalculateLookPosition();
                Debug.Log("Rotation loc target: " + pos);
                transform.LookAt(pos);
            }
            else
            {
                // 3d
                oobiModel.SetActive(true);
                oobiSprite.SetActive(false);
            }
        }

        private Vector3 CalculateLookPosition()
        {
            // get inverse forward of the camera & add that to the sprites pos
            Vector3 inverseCamForward = camera.transform.forward * -5;
            return transform.position + inverseCamForward;
        }
    }
}
