using UnityEngine;
using System;

namespace LevelObjects
{
    [ExecuteInEditMode]
    public class Rail : MonoBehaviour
    {
        // Represents a rail that can have an attached object move across it
        [Header("Rail Settings")] [SerializeField]
        private float railLength = 1; // implicitly determine start/end points

        public Vector3 RailStart { get; private set; }
        public Vector3 RailEnd { get; private set; }

        public bool IsConnected { get; private set; } = false;

        [Header("Rail Mesh")]
        [SerializeField] private GameObject railMesh;

        private Rail connectedRail = null;
        
        // events for the rail hook to listen for
        public event Action<Rail> OnRailConnected;
        public event Action<Rail> OnRailDisconnected;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            RailStart = transform.position;
            RailEnd = transform.position + (transform.forward * railLength);
            railMesh = transform.GetChild(0).gameObject;
        }
    
        public Vector3 GetPointOnRail(float t)
        {
            // takes in a value t and returns the position of the object on the rail
        
            return Vector3.Lerp(RailStart, RailEnd, t);
        }

        private void OnValidate()
        {
            // use this to adjust the length of the mesh
            railMesh.transform.localScale = new Vector3(1, 1, railLength);
            // length also = distance from centre
            // dist = 1/2 of length
        
            railMesh.transform.localPosition = new Vector3(0, 0, railLength/2);
            RailStart = transform.position;
            RailEnd = transform.position + (transform.forward * railLength);
        }

        public float GetTValueOfPoint(Vector3 point)
        {
            // we can ignore the y value of the point vector, just focus on x/z
            // get local equivalent
            Vector3 localPoint = transform.InverseTransformPoint(point);
            localPoint.y = 0;
            var localStart = transform.InverseTransformPoint(RailStart);
            var localEnd = transform.InverseTransformPoint(RailEnd);
            
            // find out where on the start -> end is. since point is between, we can work based on magnitude
            
            return 0;
        }

        public void AddRailAtEnd(Rail newRail)
        {
            if (IsConnected) return;
        }

        public void AddRailAtStart(Rail newRail)
        {
            if (IsConnected) return;
        }

        public void RemoveConnectedRail(RailData restorationData)
        {
            // reset the start / end points of the rail
            RailStart = restorationData.railStart;
            RailEnd = restorationData.railEnd;
            
            // TODO: FIRE EVENT FOR DISCONNECT
            OnRailDisconnected?.Invoke(connectedRail);
            connectedRail = null;
            IsConnected = false;
        }

        public float GetRailLength() => railLength;

        private void RailWasConnected(Rail parentRail)
        {
            // the parentRail acts as the main backend for the rail until the disconnect, and we need to set the flag
            // so that this rail won't try connecting to others
            IsConnected = true;
            connectedRail = parentRail;
        }
    }
}
