using UnityEngine;
using UnityEngine.Events;

namespace LevelObjects.Switches
{
    public abstract class ASwitch : MonoBehaviour
    {
        [SerializeField] public UnityEvent onSwitchEnabled;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        protected void RaiseSwitchEnabledEvent()
        {
            onSwitchEnabled.Invoke();
        }
    }
}
