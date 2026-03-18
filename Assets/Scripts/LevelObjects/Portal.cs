using System;
using System.Collections.Generic;
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
        
        private HashSet<GameObject> expectedObjects = new HashSet<GameObject>();
        private HashSet<GameObject> visualClones = new HashSet<GameObject>();
        
        public RenderTexture portalCameraOutput { get; private set; }

        public static event Action<Portal> OnPlayerLeftPortal;
        
        
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

        private void OnTriggerEnter(Collider other)
        {
            // make sure that the detected object is being sent through to this portal
            if (expectedObjects.Contains(other.gameObject))
                return;
            
            Vector3 relativeTranslation = other.transform.position - transform.position;
            // calculate the angles between on each axis individually
            float xAngle = Vector3.Angle(new Vector3(transform.forward.x, 0 , 0),
                new Vector3(relativeTranslation.x, 0, 0));
            
            float yAngle = Vector3.Angle(new Vector3(0, transform.forward.y, 0),
                new Vector3(0, relativeTranslation.y, 0));
            
            float zAngle = Vector3.Angle(new Vector3(0, 0, transform.forward.z),
                new Vector3(0, 0, relativeTranslation.z));
            
            Debug.Log("Relative translation of object: " + relativeTranslation);
            linkedPortal.ReceiveObject(other.gameObject, relativeTranslation, new Vector3(xAngle, yAngle, zAngle));
        }

        private void OnTriggerExit(Collider other)
        {
            expectedObjects.Remove(other.gameObject);
        }
        
        private void ReceiveObject(GameObject target, Vector3 relativeTranslation, Vector3 relativeEulerRotation)
        {
            expectedObjects.Add(target);
            // set position of the target
            target.transform.position = transform.position + relativeTranslation;
            
            // reflect the rotation of the object that passed through
            target.transform.rotation = Quaternion.Euler(Vector3.Reflect(relativeEulerRotation, transform.forward));
            
            // if the object has a rigidbody, reflect the linear velocity & forces
            Rigidbody r;
            ConstantForce cf;

            if (target.TryGetComponent<Rigidbody>(out r))
            {
                r.linearVelocity = Vector3.Reflect(r.linearVelocity, transform.forward);
            }

            if (target.TryGetComponent<ConstantForce>(out cf) && !target.CompareTag("Player"))
            {
                cf.relativeForce = Vector3.Reflect(cf.relativeForce, transform.forward);
                cf.force = Vector3.Reflect(cf.force, transform.forward);
            }
            
            if (target.CompareTag("Player"))
                OnPlayerLeftPortal?.Invoke(this);
        }
    }
}
