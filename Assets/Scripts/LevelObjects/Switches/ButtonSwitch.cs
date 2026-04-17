using UnityEngine;
using System;
using UnityEngine.Events;

namespace LevelObjects.Switches
{
    public class ButtonSwitch : ASwitch
    {
        [Header("Button Settings")] [SerializeField]
        protected float pressCooldown;

        protected float nextPressTime = 0;

        private void Start()
        {
            // add subscriptions here for making sure that we can press the button when facing down
            RaycastTriggerTarget.RaycastTriggered += OnRaycastTrigger;
        }

        private void OnDestroy()
        {
            RaycastTriggerTarget.RaycastTriggered -= OnRaycastTrigger;
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            // grab the tag of the colliding obj
            if (Array.IndexOf(validTags, other.tag) != -1)
            {
                // check cooldown is elapsed
                if (Time.time >= nextPressTime)
                {
                    // invoke switch enabled event
                    onSwitchEnabled.Invoke();
                    nextPressTime = Time.time + pressCooldown;
                }
            }
        }

        private void OnRaycastTrigger(RaycastCollision col)
        {
            if (Array.IndexOf(validTags, col.SourceGamemeObject.tag) != -1)
            {
                // check cooldown is elapsed
                if (Time.time >= nextPressTime)
                {
                    // invoke switch enabled event
                    onSwitchEnabled.Invoke();
                    nextPressTime = Time.time + pressCooldown;
                }
            }
        }
    }
}
