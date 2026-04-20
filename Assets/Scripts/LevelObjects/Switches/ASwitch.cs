using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using GameConstants.Enumerations;
using Managers;
using UnityEngine.UIElements;

namespace LevelObjects.Switches
{
    public abstract class ASwitch : MonoBehaviour
    {
        [Header("Common Settings")]
        [SerializeField] public UnityEvent onSwitchEnabled;
        [SerializeField] protected string[] validTags;
        // event listener for the dimension switch to adjust colliders
    }
}
