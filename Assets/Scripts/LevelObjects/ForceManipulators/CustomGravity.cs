using UnityEngine;

namespace LevelObjects.ForceManipulators
{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(ConstantForce))]
    public class CustomGravity : MonoBehaviour
    {
        // implements a custom gravity direction and/or magnitude to the target GameObject
        [Header("Gravity Settings")] 
        [SerializeField] private bool useGlobalGravityMagnitude = true;
        [SerializeField] private Vector3 gravityDirection = Vector3.down;
        
        // internal references
        private ConstantForce force;
        private Rigidbody objectRb;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            force = GetComponent<ConstantForce>();
            objectRb = GetComponent<Rigidbody>();
            
            // apply the custom gravity
            force.force = useGlobalGravityMagnitude? 
                gravityDirection.normalized * Physics.gravity.magnitude : gravityDirection;
        }

        public void SetGravityDirection(Vector3 newGravityDirection)
        {
            gravityDirection = newGravityDirection;
            force.force = useGlobalGravityMagnitude? 
                gravityDirection.normalized * Physics.gravity.magnitude : gravityDirection;
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
            objectRb.useGravity = false;
        }

        private void OnDisable()
        {
            objectRb.useGravity = true;
        }
    }
}
