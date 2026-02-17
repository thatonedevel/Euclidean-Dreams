using UnityEngine;
using UnityEngine.Events;

namespace LevelObjects.Switches
{
    public abstract class ASwitch : MonoBehaviour
    {
        [SerializeField] public UnityEvent onSwitchEnabled;
        [SerializeField] protected string[] validTags;
    }
}
