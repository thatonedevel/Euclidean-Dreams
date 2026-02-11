using System;
using UnityEngine;
using GameConstants;
using Data;

namespace LevelObjects
{
    public class LevelGoal : MonoBehaviour
    {
        public static event Action OnGoalReached;

        // reference to the current level's LevelData instance
        [SerializeField] private LevelData currentLevelDataObject;

        public void OnTriggerEnter(Collider collision)
        {
            Debug.Log("col detected");
            if (collision.CompareTag(Constants.TAG_PLAYER))
            {
                Debug.Log("Player entered goal");
                currentLevelDataObject.StopLevelTimer();
                OnGoalReached?.Invoke();
            }
        }
    }
}
