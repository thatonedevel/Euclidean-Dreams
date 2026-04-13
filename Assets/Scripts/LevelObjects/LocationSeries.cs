using UnityEngine;
using System.Collections.Generic;
using System;
using Vector3 = UnityEngine.Vector3;

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
        [SerializeField] private bool startAutomatically = false;

        private float neededLerpTime = 0;
        private bool isMoving = false;
        private Vector3 currentLerpStartLocation = Vector3.zero;
        private float currentLerpTime = -1;
        
        private const float DISTANCE_THRESHOLD = 0.1f;
        
        // sends an event once a location is reached
        public event Action<int, Vector3> OnDestinationReached; 
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (startAutomatically)
                StartMovingToNextLocation();
        }

        // Update is called once per frame
        void Update()
        {
            if (isMoving)
            {
                // calculate the current t value for the lerp
                // t = current lerp time / needed
                float t = currentLerpTime / neededLerpTime;
                transform.position = Vector3.Lerp(currentLerpStartLocation, destinations[destinationIndex], t);
                
                // check for the snap
                if (Vector3.Distance(transform.position, destinations[destinationIndex]) <= DISTANCE_THRESHOLD || t >= 1)
                {
                    Debug.Log("Snapping to destination");
                    // snap to destination
                    isMoving = false;
                    transform.position = destinations[destinationIndex];
                    if (loop)
                        destinationIndex = destinationIndex >= destinations.Count - 1 ? 0 : destinationIndex + 1;
                    else
                    {
                        destinationIndex++;
                        if (destinationIndex >= destinations.Count) enabled = false;
                    }
                    
                    if (continuous)
                    {
                        StartMovingToNextLocation();
                    }
                }
                else
                {
                    // make sure we update the current lerp time
                    currentLerpTime += Time.deltaTime;
                }
            }
        }

        public void StartMovingToNextLocation()
        {
            Debug.Log("moving to next lerp destination");
            // if the object is idle, start moving it to the next location in the array
            if (!isMoving)
            {
                // set lerp time to 0
                currentLerpTime = 0;
                // calculate the needed lerp time (t = dist / speed)
                neededLerpTime = Vector3.Distance(transform.position, destinations[destinationIndex]);
                isMoving = true;
                currentLerpStartLocation = transform.position;
            }
            else
            {
                Debug.Log("already moving");
            }
        }

        public void StopMovingToNextLocation()
        {
            isMoving = false;
            neededLerpTime = -1;
            currentLerpTime = -1;
        }
    }
}
