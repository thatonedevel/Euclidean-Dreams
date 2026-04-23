using LevelObjects.ForceManipulators;
using UnityEngine;
using UnityEngine.InputSystem;
using GameConstants;
using GameConstants.Enumerations;

namespace Animation
{
    public class PlayerAnimationControl : MonoBehaviour
    {
        [SerializeField] protected Animator pcAnimator;
        [SerializeField] protected Rigidbody pcRigidbody;
        [SerializeField] protected CustomGravity customGrav;
        
        // add movement action listening here
        private InputAction moveAction;
        
        // animation parameters
        const string PARAM_MOVING = "IsMoving";
        const string PARAM_FALLING = "IsFalling";
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected void Start()
        {
            moveAction = InputSystem.actions.FindAction(Constants.ACTION_MOVE);
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            // check the state of the rigidbody to determine what state we need to update
            Vector3 velocity = pcRigidbody.linearVelocity;
            Vector3 grav = customGrav.GetGravityDirection();

            pcAnimator.SetBool(PARAM_MOVING, moveAction.ReadValue<Vector2>().magnitude > 0);
            
            // check if local vel y < 0 => falling
            pcAnimator.SetBool(PARAM_FALLING, transform.parent.InverseTransformDirection(velocity).y < 0);
        }
    }
}
