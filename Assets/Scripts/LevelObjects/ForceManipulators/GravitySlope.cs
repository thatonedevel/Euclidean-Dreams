using UnityEngine;

namespace LevelObjects.ForceManipulators
{
    public class GravitySlope : AGravityManipulator
    {
        [Header("Slope Settings")] [SerializeField]
        private Transform slopeNormal;
        
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Debug.Log("Collision Enter");
                // if player set custom gravity to follow the direction of the slope
                ContactPoint cont = collision.GetContact(0); // 0th contact's collider is this object
                
                // set gravity to use the down direction of the slope's normal transform
                var mov = collision.gameObject.GetComponent<CharacterMovement>();
                
                mov.SetCustomGravity(slopeNormal.transform.up * Physics.gravity.magnitude * -1);
                // also make sure to adjust the rotation of the player on the necessary axes
            }
        }
    }
}
