using UnityEngine;
using System;
using UnityEditor;
using UnityEngine.Diagnostics;

namespace LevelObjects.ForceManipulators
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(ConstantForce))]
    public class CustomGravity : MonoBehaviour
    {
        // implements a custom gravity direction and/or magnitude to the target GameObject
        [Header("Gravity Settings")] 
        [SerializeField] private bool useGlobalGravityMagnitude = true;
        [SerializeField] private Vector3 gravityDirection = Vector3.down;
        
        // event fired when this has a new gravity direction set
        public event Action<Vector3> OnGravityDirectionChanged;
        public event Action OnGravityRestored;
        
        // internal references
        private ConstantForce force;
        private Rigidbody objectRb;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        private void Start()
        {
            // check if we need to set references to the components
            if (objectRb == null)
                objectRb = GetComponent<Rigidbody>();
            if (force == null)
                force = GetComponent<ConstantForce>();
            
            objectRb.useGravity = false;
            // set the constant force to use the provided gravity direction
            force.force =  useGlobalGravityMagnitude? gravityDirection.normalized * Physics.gravity.magnitude : gravityDirection;
        }
        
        public void SetGravityDirection(Vector3 newGravityDirection)
        {
            gravityDirection = newGravityDirection;
            force.force = useGlobalGravityMagnitude? 
                gravityDirection.normalized * Physics.gravity.magnitude : gravityDirection;
            
            // adjust the directional vectors of the transform
            transform.up = newGravityDirection.normalized * -1;
            objectRb.useGravity = false;
            // we read the gravity as a down direction, so from here, calculate the left & forward directions
            // enable self on call
            force.enabled = true;
            OnGravityDirectionChanged?.Invoke(newGravityDirection);
        }

        public void SetUseGlobalGravityMagnitude(bool newVal)
        {
            useGlobalGravityMagnitude = newVal;
        }
        
        // getters
        public Vector3 GetGravityDirection()
        {
            return gravityDirection;
        }

        public bool GetUseGlobalGravityMagnitude()
        {
            return useGlobalGravityMagnitude;
        }

        private void OnEnable()
        {
            // check if we need to set references to the components
            if (objectRb == null)
                objectRb = GetComponent<Rigidbody>();
            if (force == null)
                force = GetComponent<ConstantForce>();
            
            objectRb.useGravity = false;
            // set the constant force to use the provided gravity direction
            force.force =  useGlobalGravityMagnitude? gravityDirection.normalized * Physics.gravity.magnitude : gravityDirection;
        }

        private void OnDisable()
        {
            objectRb.useGravity = true;
            force.enabled = false;
            
            OnGravityRestored?.Invoke();
        }
    }
}
