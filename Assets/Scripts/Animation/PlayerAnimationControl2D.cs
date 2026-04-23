using UnityEngine;
using UnityEngine.InputSystem;

namespace Animation
{
    public class PlayerAnimationControl2D : PlayerAnimationControl
    {
        [Header("2D Animation References")]
        [SerializeField] private SpriteRenderer oobiRenderer;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            // check if we need to have the sprite flipped
            oobiRenderer.flipX = (int)moveAction.ReadValue<Vector2>().x != 0? moveAction.ReadValue<Vector2>().x > 0 : oobiRenderer.flipX;
        }
    }
}
