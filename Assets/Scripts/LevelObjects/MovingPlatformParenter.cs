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
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.TAG_PLAYER))
            {
                other.transform.SetParent(null);
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
