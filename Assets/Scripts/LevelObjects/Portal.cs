using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Experimental.Rendering;

namespace LevelObjects
{
    public class Portal : MonoBehaviour
    {
        [Header("Portal Components")]
        [SerializeField] private Camera portalCamera;
        [SerializeField] private Portal linkedPortal;
        [SerializeField] private MeshRenderer planeRenderer;
        
        public RenderTexture portalCameraOutput { get; private set; }


        private void Awake()
        {
            // initialise materials & out texture here
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        
        void Start()
        {
            // if the linked portal exists, grab its output render texture
        }

        private void InitialisePortal()
        {
            // make a new material & render texture that is displayed on the output plane
            Material planeMaterial = new Material(Shader.Find("Standard"));
            RenderTexture output = new RenderTexture(128, 256, 24);

            portalCameraOutput = output;
            portalCamera.targetTexture = output;
            planeMaterial.SetTexture("_MainTex", output);
            // set the emission
            planeMaterial.SetTexture("_EmissionColorMap", output);
            planeMaterial.SetTexture("_EmissiveColor", Texture2D.whiteTexture);
        }
    }
}
