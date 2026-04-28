using System;
using UnityEngine;
using UnityEngine.Events;

namespace LevelObjects.Switches
{
    public class PressureButton : ButtonSwitch
    {
        // button that executes its assigned callbacks as long as it is pressed with a given frequency
        [Header("Pressure Button Settings")] [SerializeField]
        private float callbackInterval = 1;
        private float nextCallbackTime = 0;
        public UnityEvent OnSwitchReleased;

        private bool isPressed = false;

        protected override void OnTriggerEnter(Collider other)
        {
            // in this case set the ispressed flag to true, & handle invocation as part of the fixedupdate
            // we also run an immediate action once pressed
            base.OnTriggerEnter(other);
            isPressed = true;
            nextCallbackTime = Time.time + callbackInterval;
        }

        private void OnTriggerExit(Collider other)
        {
            isPressed = false;
            nextPressTime = Time.time + pressCooldown;
            OnSwitchReleased.Invoke();
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
