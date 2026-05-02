using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace LevelObjects.Switches
{
    public class PressureButton : ButtonSwitch
    {
        // button that executes its assigned callbacks as long as it is pressed with a given frequency
        [Header("Pressure Button Settings")] [SerializeField]
        private float callbackInterval = 1;
        private float nextCallbackTime = 0;
        public UnityEvent OnSwitchReleased;

        private Dictionary<string, bool> tagCheckDict = new();
        
        private bool isPressed = false;

        protected override void Start()
        {
            base.Start();
            // set up the dictionary with the allowed tags
            for (int i = 0; i < validTags.Length; i++)
            {
                tagCheckDict.Add(validTags[i], false);
            }
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            // in this case set the ispressed flag to true, & handle invocation as part of the fixedupdate
            // we also run an immediate action once pressed
            base.OnTriggerEnter(other);
            isPressed = true;
            nextCallbackTime = Time.time + callbackInterval;
            if (Array.IndexOf(validTags, other.tag) != -1)
                tagCheckDict[other.tag] = true;
        }

        private void OnTriggerExit(Collider other)
        {
            // check to see if anything else is pressing it

            if (tagCheckDict.ContainsKey(other.tag))
            {
                tagCheckDict[other.tag] = false;
            }
            
            isPressed = tagCheckDict.Values.Any(x => x);

            if (!isPressed)
            {
               nextPressTime = Time.time + pressCooldown;
               OnSwitchReleased.Invoke(); 
            }
        }
        
        private void FixedUpdate()
        {
            // check if the button is pressed & we have hit the time threshold
            if (isPressed && Time.time >= nextCallbackTime)
            {
                onSwitchEnabled.Invoke();
                nextCallbackTime = Time.time + callbackInterval;
            }
        }
    }
}
