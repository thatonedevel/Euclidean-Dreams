using GameConstants;
using UnityEngine;

namespace LevelObjects
{
    public class MovingPlatformParenter : MonoBehaviour
    {
        private void Start()
        {
            // subscribe to the raycast events
            RaycastTriggerTarget.RaycastTriggered += RaycastTriggerEnterHandler;
            RaycastTriggerTarget.RaycastTriggerExit += RaycastTExitHandler;
        }

        private void OnDestroy()
        {
            RaycastTriggerTarget.RaycastTriggered -= RaycastTriggerEnterHandler;
            RaycastTriggerTarget.RaycastTriggerExit -= RaycastTExitHandler;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.TAG_PLAYER))
            {
                other.transform.SetParent(transform);
                
                var playerRb = other.GetComponent<Rigidbody>();
                
                // (Carry the Sword Game Studio, 2021)
                // set interpolation to non here to prevent jittery movement
                playerRb.interpolation = RigidbodyInterpolation.Interpolate;
                playerRb.linearVelocity = Vector3.zero;
                playerRb.isKinematic = true;
                
                Debug.Log("Player entered");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.TAG_PLAYER))
            {
                other.transform.SetParent(null);
                var playerRb = other.GetComponent<Rigidbody>();
                playerRb.interpolation = RigidbodyInterpolation.None;
                playerRb.isKinematic = false;
            }
        }

        private void RaycastTriggerEnterHandler(RaycastCollision collision)
        {
            if (collision.SourceGamemeObject.CompareTag(Constants.TAG_PLAYER))
            {
                collision.SourceGamemeObject.transform.SetParent(transform);
            }
        }
        
        private void RaycastTExitHandler(RaycastCollision collision)
        {
            if (collision.SourceGamemeObject.CompareTag(Constants.TAG_PLAYER))
            {
                collision.SourceGamemeObject.transform.SetParent(null);
            }
        }
    }
}
