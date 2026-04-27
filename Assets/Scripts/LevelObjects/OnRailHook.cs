using System;
using GameConstants.Enumerations;
using UnityEditor.Build.Pipeline.WriteTypes;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace LevelObjects
{
    [ExecuteInEditMode]
    public class OnRailHook : MonoBehaviour
    {
        // object that moves across a rail. acts as a container for other objects that will be placed onto this as children
        [SerializeField] private Rail parentRail;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float snapThreshold = 0.1f;
        [SerializeField] [Range(0, 1)] private float t = 0;

        private float tDelta = 0;

        private bool queueResubscription = false;
        private Rail reParentRail;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // calculate how much t should change per frame that the move methods are called
            var length = parentRail.GetRailLength();
            tDelta = length * moveSpeed;
            transform.position = parentRail.GetPointOnRail(t);
            parentRail.OnRailDisconnected += OnRailDisconnected;
            parentRail.OnRailConnected += OnRailConnected;
        }

        private void Update()
        {
            if (queueResubscription)
            {
                // unsub from parent rail events
                parentRail.OnRailDisconnected -= OnRailDisconnected;
                parentRail.OnRailConnected -= OnRailConnected;
                // set new parent and sub to its disconnect event
                parentRail = reParentRail;
                transform.parent = reParentRail.transform;
                
                // sub to new rail events
                parentRail.OnRailDisconnected += OnRailDisconnected;
                parentRail.OnRailConnected += OnRailConnected;
                
                t =  parentRail.GetTValueOfPoint(transform.position);
                transform.position = parentRail.GetPointOnRail(t);
                queueResubscription = false;

                reParentRail = null;
            }
        }

        private void OnDestroy()
        {
            parentRail.OnRailDisconnected -= OnRailDisconnected;
        }
        
        public void MoveForward()
        {
            // increase t and update pos
            t += tDelta * Time.deltaTime;
            // if t is close to 1, snap it
            t = t >= 1 - snapThreshold ? 1 : t;
            transform.position = parentRail.GetPointOnRail(t);
        }

        public void MoveBackwards()
        {
            // decrease t and update pos
            t -= tDelta * Time.deltaTime;
            t = t <= snapThreshold? 0 : t;
            transform.position = parentRail.GetPointOnRail(t);
        }

        private void OnValidate()
        {
            if (parentRail == null)
                return;
            // use this to set the position of the object when t is adjusted
            transform.position = parentRail.GetPointOnRail(t);
        }

        private void OnRailConnected(Rail target)
        {
            // calculate the new t value
            t =  parentRail.GetTValueOfPoint(transform.position);
            transform.position = parentRail.GetPointOnRail(t);
        }
        
        private void OnRailDisconnected(Rail disconTarget)
        {
            // get the t value of parent
            float tempT = parentRail.GetTValueOfPoint(transform.localPosition, true);

            if (tempT >= snapThreshold)
            {
                t = tempT > 1 ? 1 : tempT;
                transform.position = parentRail.GetPointOnRail(t);
                Debug.Log("staying on current rail");
            }
            else
            {
                queueResubscription = true;
                reParentRail = disconTarget;
            }
        }
    }
}