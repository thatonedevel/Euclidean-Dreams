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
                // check that the player's position is valid
                if (IsPlayerPositionValid(other.gameObject))
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

        private bool IsPlayerPositionValid(GameObject player)
        {
            // called to check that the player is in a valid position to be parented (ie below if upside down & vice versa)
            bool valid = true;

            Vector3 playerDownDir = player.transform.up * -1;
            Vector3 localPlayerPos = transform.InverseTransformPoint(player.transform.position);
            Vector3 downDir = transform.up * -1;

            if (playerDownDir == downDir)
            {
                // down direction matches, so check if player is above platform locally
                valid = localPlayerPos.y > transform.localPosition.y;
            }
            else
            {
                valid =  localPlayerPos.y < transform.localPosition.y;
            }
            
            return valid;
        }
    }
}
