using System;
using UnityEngine;
using GameConstants;

public class LevelGoal : MonoBehaviour
{
    public static event Action OnGoalReached;

    public void OnTriggerEnter(Collider collision)
    {
        Debug.Log("col detected");
        if (collision.CompareTag(Constants.TAG_PLAYER))
        {
            Debug.Log("Player entered goal");
            OnGoalReached?.Invoke();
        }
    }
}
