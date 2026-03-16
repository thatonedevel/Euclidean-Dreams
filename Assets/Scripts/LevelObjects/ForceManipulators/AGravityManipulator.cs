using System;
using UnityEngine;

namespace LevelObjects.ForceManipulators
{
    public abstract class AGravityManipulator : MonoBehaviour
    {
        [Header("Gravity Snap Points")]
        [SerializeField] protected Transform snapPoint1;
        [SerializeField] protected Transform snapPoint2;
        
        protected virtual void OnTriggerExit(Collider other)
        {
            // snap the gravity of the player to the closest point
            if (other.gameObject.CompareTag("Player"))
            {
                // if we're not in range of either exit snap point, don't remove the custom gravity
                if (Vector3.Distance(other.transform.position, snapPoint1.position) > 0.1f &&
                    Vector3.Distance(other.transform.position, snapPoint2.position) > 0.1f && Vector3.Distance(transform.position, other.transform.position) < 1)
                {
                    return;
                }
                
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
