using UnityEngine;

namespace LevelObjects
{
    public class Portal : MonoBehaviour
    {
        [Header("Portal Components")]
        [SerializeField] private Camera portalCamera;
        [SerializeField] private Portal linkedPortal;
        [SerializeField] private MeshRenderer planeRenderer;
        
        public RenderTexture portalCameraOutput { get; private set; }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // create a new render texture which our camera writes to
            portalCameraOutput = new RenderTexture(Screen.width, Screen.height, 24);

            if (linkedPortal != null)
            {
                // make the plane attached to this portal render the output from the other portal
                planeRenderer.material.mainTexture = linkedPortal.portalCameraOutput;
            }
        }
    }
}
