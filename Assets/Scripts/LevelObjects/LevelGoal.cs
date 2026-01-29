using System;
using UnityEngine;
using GameConstants;

public class LevelGoal : MonoBehaviour
{
    public static event Action OnGoalReached;

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag(Constants.TAG_PLAYER))
        {
            OnGoalReached?.Invoke();
        }
    }
}
