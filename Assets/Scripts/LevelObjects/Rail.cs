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
        public bool IsStartRail { get; private set; } = true;

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
            // if we're connected to another rail, consider the full rail length
            if (!IsConnected)
                return Vector3.Lerp(RailStart, RailEnd, t);
            else if (IsStartRail)
                return Vector3.Lerp(RailStart, connectedRail.RailEnd, t); // use local start + end of connected rail
            else
                return Vector3.Lerp(connectedRail.RailStart,  RailEnd, t); // use other start and local end
            
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

        public float GetTValueOfPoint(Vector3 point, bool isLocal=false)
        {
            // we can ignore the y value of the point vector, just focus on x/z
            // get local equivalent
            
            Vector3 localPoint = isLocal? point : transform.InverseTransformPoint(point);
            localPoint.y = 0;
            localPoint.x = 0;
            var localStart = transform.InverseTransformPoint(RailStart);
            var localEnd = transform.InverseTransformPoint(RailEnd);

            if (IsConnected)
            {
                if (IsStartRail)
                {
                    localEnd = transform.InverseTransformPoint(connectedRail.RailEnd);
                }
                else
                {
                    localStart = transform.InverseTransformPoint(connectedRail.RailStart);
                }
            }
            
            // find out where on the start -> end is. since point is between, we can work based on magnitude
            float distToPoint = Vector3.Distance(localPoint, localStart);
            
            // t = fraction of point dist / total
            
            return distToPoint / railLength;
        }

        public void AddRailAtEnd(Rail newRail)
        {
            if (IsConnected) return;
            // we're the "original" rail
            connectedRail = newRail;
            IsConnected = true;
            IsStartRail = true;
            connectedRail.RailWasConnected(this);
            OnRailConnected?.Invoke(connectedRail);
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

        public float GetRailLength()
        {
            if (IsConnected)
            {
                return railLength + connectedRail.railLength; // give total length if rails are connected
            }
            return railLength;
        }

        private void RailWasConnected(Rail parentRail)
        {
            // the parentRail acts as the main backend for the rail until the disconnect, and we need to set the flag
            // so that this rail won't try connecting to others
            IsConnected = true;
            connectedRail = parentRail;
            IsStartRail = false;
        }
    }
}
