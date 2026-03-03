using Data.Saves;
using GameConstants.Enumerations;
using UnityEngine;
using LevelObjects;
using Managers;
using Unity.VisualScripting;
using System;

namespace Data
{
    public class LevelData : MonoBehaviour
    {
        // class used to store data about the current level
        // stores collectible status & level clear time

        // all the data we want to read
        public float levelClearTime { get; private set; } = -1;
        public bool[] gemCollectionStatus = new bool[3];
        [SerializeField] private string stageName;

        // internal data
        private float levelStartTime;
        
        // event raised once level is initialised
        public static event Action<bool[]> OnLevelInitComplete;

        public string GetStageName() => stageName;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // subscribe to gem collected event
            Gem.OnGemCollected += GemCollectedHandler;
            levelStartTime = Time.time;
            LevelInit();
        }

        private void OnDestroy()
        {
            // unsubscribe from events
            Gem.OnGemCollected -= GemCollectedHandler;
        }

        public void ResetLevelTime()
        {
            // resets the level start time to current time

        }

        public void ResetLevelData()
        {

        }

        private void GemCollectedHandler(GemOrders num)
        {
            // gemorders is an enum so we can cast directly to an int to get the index
            gemCollectionStatus[(int)num] = true;
        }

        public void StopLevelTimer()
        {
            // call this when we want to get the stage's clear time
            levelClearTime = Mathf.Round(Time.time - levelStartTime);
        }

        private void LevelInit()
        {
            // get index of this stage
            int index = GameController.Singleton.CurrentLevelIndex;

            bool[] gems = SaveDataManager.Singleton.GetGemStatusForStage(index);

            if (gems.Length == 3)
            {
                // update gem collection status 
                gemCollectionStatus[0] = gems[0];
                gemCollectionStatus[1] = gems[1];
                gemCollectionStatus[2] = gems[2];
            }
            
            OnLevelInitComplete?.Invoke(gems);
        }
    }
}