using UnityEngine;
using UnityEngine.Events;

namespace LevelObjects.Switches
{
    public abstract class ASwitch : MonoBehaviour
    {
        [Header("Common Settings")]
        [SerializeField] public UnityEvent onSwitchEnabled;
        [SerializeField] protected string[] validTags;
    }
}
