using LevelObjects.ForceManipulators;
using UnityEngine;

namespace Animation
{
    public class PlayerAnimationControl : MonoBehaviour
    {
        [SerializeField] private Animator pcAnimator;
        [SerializeField] private Rigidbody pcRigidbody;
        [SerializeField] private CustomGravity customGrav;
        
        // animation parameters
        const string PARAM_MOVING = "IsMoving";
        const string PARAM_FALLING = "IsFalling";
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            // check the state of the rigidbody to determine what state we need to update
            Vector3 velocity = pcRigidbody.linearVelocity;
            Vector3 grav = customGrav.GetGravityDirection();

            pcAnimator.SetBool(PARAM_MOVING, velocity.magnitude > 0);
            
            // check if we're falling
            if (customGrav.enabled)
            {
                // if local vel y < 0, we are falling
                pcAnimator.SetBool(PARAM_FALLING, 
                    transform.parent.InverseTransformDirection(velocity).y < 0);
            }
            else
            {
                pcAnimator.SetBool(PARAM_MOVING, false);
            }
        }
    }
}
