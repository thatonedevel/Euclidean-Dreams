using UnityEngine;

namespace LevelObjects.ForceManipulators
{
    public class GravitySlope : AGravityManipulator
    {
        [Header("Slope Settings")] [SerializeField]
        private Transform slopeNormal;
        
        private void OnTriggerStay(Collider collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                var mov = collision.gameObject.GetComponent<CharacterMovement>();
                mov.SetCustomGravity(slopeNormal.transform.up * Physics.gravity.magnitude * -1);
            }
        }
    }
}
