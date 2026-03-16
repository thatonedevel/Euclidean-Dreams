using System;
using UnityEngine;

namespace LevelObjects.ForceManipulators
{
    public abstract class AGravityManipulator : MonoBehaviour
    {
        [Header("Gravity Directions")]
        [SerializeField] protected Vector3 direction1 = Vector3.zero;
        [SerializeField] protected Vector3 direction2 = Vector3.zero;

        [Header("Gravity Snap Points")]
        [SerializeField] protected Transform snapPoint1;
        [SerializeField] protected Transform snapPoint2;
        
        private void OnCollisionExit(Collision other)
        {
            // snap the gravity of the player to the closest point
            if (other.gameObject.CompareTag("Player"))
            {
                var mov = other.gameObject.GetComponent<CharacterMovement>();
                
                Vector3 newGravityAcceleration = Vector3.Distance(snapPoint1.transform.position, other.transform.position) <
                                                 Vector3.Distance(snapPoint2.transform.position, other.transform.position) ? 
                    snapPoint1.transform.up *-1 : snapPoint2.transform.up * -1;
                
                // check if this is approximate to normal gravity (i.e. -9.81 on the y axis)
                if (Vector3.Distance(newGravityAcceleration, Physics.gravity) <= 0.1f)
                {
                    // use default gravity
                    mov.DisableCustomGravity();
                }
                else
                {
                    mov.SetCustomGravity(newGravityAcceleration);
                }
            }
        }
    }
}
