using UnityEngine;
using System.Collections.Generic;

namespace LevelObjects
{
    public class LocationSeries : MonoBehaviour
    {
        // allows an object to move smoothly between a series of defined locations
        [Header("Settings")] [SerializeField] private List<Vector3> destinations = new();
        [SerializeField] private float movementSpeed = 5.0f;
        [SerializeField] private bool loop = false;
        [SerializeField] private int destinationIndex = 0;
        [SerializeField] private bool continuous = false; // do we keep moving between points when we reach the current target?

        private float neededLerpTime = 0;
        private bool isMoving = false;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                
            }
        }

        public void StartMovingToNextLocation()
        {
            // if the object is idle, start moving it to the next location in the array
            if (!isMoving)
            {
                // calculate the needed lerp time (t = dist / speed)
                neededLerpTime = Vector3.Distance(transform.position, destinations[destinationIndex]);
                isMoving = true;
            }
            else
            {
                Debug.Log("already moving");
            }
        }
    }
}
