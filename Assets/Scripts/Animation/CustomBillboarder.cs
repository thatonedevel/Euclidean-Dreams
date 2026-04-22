using UnityEngine;
using GameConstants.Enumerations;
using GameConstants;

namespace Animation
{
    public class CustomBillboarder : MonoBehaviour
    {
        // billboards the sprite to face towards the camera, & enables/disables the sprite or mesh when in a given perspective
        [Header("Object References")] 
        [SerializeField] private SpriteRenderer oobiSpriteRenderer;
        [SerializeField] private GameObject oobiModel;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
        }

        private void DimensionSwitchHandler(Dimensions newDim)
        {
            if (newDim == Dimensions.SECOND)
            {
                // 2d
                // enable the sprite & set the global rotation to match the camera rotation
                oobiModel.SetActive(false);
                oobiSpriteRenderer.enabled = true;

                Quaternion camRot = GameObject.FindGameObjectWithTag(Constants.TAG_CAMERA).transform.rotation;
                transform.rotation = camRot;
            }
            else
            {
                // 3d
                oobiModel.SetActive(true);
                oobiSpriteRenderer.enabled = false;
            }
        }
    }
}
