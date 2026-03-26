using System;
using System.Collections.Generic;
using GameConstants.Enumerations;
using LevelObjects.ForceManipulators;
using UnityEngine;

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
            
            // if the object has a rigidbody, reflect the linear velocity & forces
            
            if (target.CompareTag("Player"))
                OnPlayerLeftPortal?.Invoke(this);
            else
            {
                RotateObjectOnExit(target);
            }
            
            // if we have a custom gravity comp adjust it
            if (target.TryGetComponent(out CustomGravity cg))
            {
                if ((int)transform.rotation.eulerAngles.x != 0 || (int)transform.rotation.eulerAngles.z != 0)
                {
                    // non standard gravity
                    cg.enabled = true;
                    cg.SetGravityDirection(transform.up * -1);
                }
                else
                {
                    // normal mavity
                    cg.enabled = false;
                }
            }
        }

        private void RotateObjectOnExit(GameObject target)
        {
            // rotates the target object to follow the direction of the exit portal

            // get the axis we need to check
            
            
            if (DoesRotationMatchLinkedPortal())
            {
                ReflectObjectAtExit(target);
            }
            else
            {
                // we need to check if we need to adjust the relative position of the object on exit (i.e. the exit is
                // not facing straight down)

                if (IsPortalNotUpright())
                {
                    // get directional vector between us and object
                    Vector3 untDir =  target.transform.position - transform.position;
                    
                    untDir = Vector3.RotateTowards(untDir, transform.forward, 360 * Mathf.Deg2Rad, 0.1f);
                    target.transform.position = transform.position + untDir;
                    // check if the forward vectors are parallel
                    if (Vector3.Angle(transform.forward, linkedPortal.transform.forward) == 0)
                    {
                        // reflect the object
                        ReflectObjectAtExit(target);
                    }
                    else
                    {
                        RotateObjectOnExit(target);
                    }
                }
                else
                {
                    RotateObjectAtExit(target);
                }
            }
        }

        private void RotateObjectAtExit(GameObject target)
        {
            // adjusts forces active n target objects that passed through this portal
            // if we're upside down, apply needed things here
            if (target.TryGetComponent(out Rigidbody r))
            {
                r.linearVelocity = Vector3.RotateTowards(r.linearVelocity,
                    transform.forward * r.linearVelocity.magnitude, Mathf.Deg2Rad * 360, 0.1f);
            }
                
            if (target.TryGetComponent(out ConstantForce cf) && !target.CompareTag("Player"))
            {
                cf.relativeForce = Vector3.RotateTowards(cf.relativeForce, 
                    transform.forward * cf.relativeForce.magnitude, Mathf.Deg2Rad * 360, 0.1f);
                cf.force = Vector3.RotateTowards(cf.force, 
                    transform.forward * cf.force.magnitude, Mathf.Deg2Rad * 360, 0.1f);
            }
        }

        private void ReflectObjectAtExit(GameObject target)
        {
            // in this case we *reflect* the forces rather than rotating them
            if (target.TryGetComponent(out Rigidbody r))
            {
                r.linearVelocity = Vector3.Reflect(r.linearVelocity, transform.forward);
            }
                
            if (target.TryGetComponent(out ConstantForce cf) && !target.CompareTag("Player"))
            {
                cf.relativeForce = Vector3.Reflect(cf.relativeForce, transform.forward);
                cf.force = Vector3.Reflect(cf.force, transform.forward);
            }
        }

        public bool DoesRotationMatchLinkedPortal()
        {
            return transform.rotation ==  linkedPortal.transform.rotation;
        }

        public bool DoesRotationMatchLinkedPortal(Axes axisToCheck)
        {
            // checks if rotation of the portal matches on the specified axis
            bool match = false;

            switch (axisToCheck)
            {
                // check based on euler rotation
                case Axes.X:
                    match = (int)linkedPortal.transform.eulerAngles.x == (int)transform.eulerAngles.x;
                    break;
                case Axes.Y:
                    match = (int)linkedPortal.transform.eulerAngles.y == (int)transform.eulerAngles.y;
                    break;
                case Axes.Z:
                    match = (int)linkedPortal.transform.eulerAngles.z == (int)transform.eulerAngles.z;
                    break;
            }
            return match;
        }

        public bool IsPortalNotUpright()
        {
            return transform.up.normalized != Vector3.up;
        }
    }
}
