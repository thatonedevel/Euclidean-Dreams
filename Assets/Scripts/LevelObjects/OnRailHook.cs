using UnityEngine;

namespace LevelObjects
{
    public class OnRailHook : MonoBehaviour
    {
        // object that moves across a rail. acts as a container for other objects that will be placed onto this as children
        [SerializeField] private Rail parentRail;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float snapThreshold = 0.1f;

        private float tDelta = 0;
        private float t = 0;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // calculate how much t should change per frame that the move methods are called
            var length = parentRail.GetRailLength();
            tDelta = length * moveSpeed;
        }

        public void MoveForward()
        {
            // increase t and update pos
            t += tDelta * Time.deltaTime;
            // if t is close to 1, snap it
            t = t >= 1 - snapThreshold ? 1 : t;
            transform.position = parentRail.GetPointOnRail(t);
        }

        private void MoveBackwards()
        {
            // decrease t and update pos
            t -= tDelta * Time.deltaTime;
            t = t <= snapThreshold? 0 : t;
            transform.position = parentRail.GetPointOnRail(t);
        }
    }
}
