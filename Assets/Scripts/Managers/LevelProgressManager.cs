using EGUI;
using LevelObjects;
using UnityEngine;
using System;
using Data;

namespace Managers
{
    public class LevelProgressManager : MonoBehaviour
    {
        // class used to control player progression & unlock stages
        // like other managers this uses the singleton pattern
        public static LevelProgressManager Singleton { get; private set; }

        public static event Action<int, int, float, bool[]> LevelProgressUpdated;
        
        public int LastUnlockedStageIndex { get; private set; } = 0;
        public int LastUnlockedBonusStageIndex { get; private set; } = -1; // TODO: use this once we implement bonus stages

        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
            else
                Destroy(gameObject);
            
            // subscribe to events
            LevelGoal.OnGoalReached += StageCompletedHandler;
        }
        
        private void StageCompletedHandler()
        {
            // get the current stage & check if it's the last unlocked one
            int currentStage = GameController.Singleton.CurrentLevelIndex;

            if (currentStage == LastUnlockedStageIndex)
            {
                // we can unlock the next stage
                LastUnlockedStageIndex++;
            }
            
            // grab the gem info & completion time from the level data object
            var levelDat = GameObject.FindWithTag("LevelData").GetComponent<LevelData>();
            
            LevelProgressUpdated?.Invoke(LastUnlockedStageIndex, currentStage, levelDat.levelClearTime, levelDat.gemCollectionStatus);
        }
    }
}
