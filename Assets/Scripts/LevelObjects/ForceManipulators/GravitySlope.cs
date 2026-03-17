using UnityEngine;

namespace LevelObjects.ForceManipulators
{
    public class GravitySlope : AGravityManipulator
    {
        [Header("Slope Settings")] [SerializeField]
        private Transform slopeNormal;
        
        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var mov = collision.gameObject.GetComponent<CharacterMovement>();
                mov.SetCustomGravity(slopeNormal.transform.up * Physics.gravity.magnitude * -1);
                // snap player to slope center to prevent the gravity glitching
                mov.transform.position = new Vector3(mov.transform.position.x, slopeNormal.transform.position.y, mov.transform.position.z);
            }
        }
    }
}
