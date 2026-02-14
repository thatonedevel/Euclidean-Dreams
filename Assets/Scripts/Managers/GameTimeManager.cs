using GameConstants.Enumerations;
using UnityEngine;

namespace Managers
{
    public class GameTimeManager : MonoBehaviour
    {
        // class for managing play / level time
        // TODO: implement full play time with the save system
        
        public float TotalPlayTime { get; private set; }
        public float CurrentLevelTime { get; private set; }
        
        public static GameTimeManager Singleton { get; private set; }
        
        private bool updateGameTime = false;
        private bool updateLevelTime = false;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // make this a singleton class
            if (Singleton == null)
            {
                Singleton = this;
                // subscribe to the state change event
                GameController.OnGameStateChanged += GameStateUpdateHandler;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // if the timers are enabled, then update them
            if (updateGameTime) TotalPlayTime += Time.deltaTime;
            
            if (updateLevelTime) CurrentLevelTime += Time.deltaTime;
        }

        public void ResetAllTimers()
        {
            // resets all timers & stops them
            updateGameTime = false;
            updateLevelTime = false;
            
            TotalPlayTime = 0;
            CurrentLevelTime = 0;
        }


        public void ResetLevelTimer()
        {
            updateLevelTime = false;
            CurrentLevelTime = 0;
        }
        
        public void StartLevelTimer()
        {
            updateLevelTime = true;
        }

        public void StopLevelTimer()
        {
            
        }

        public void StopGameTimer()
        {
            
        }

        private void GameStateUpdateHandler(GameStates newState, GameStates oldState)
        {
            switch (newState)
            {
                case GameStates.PLAYING:
                    ResetLevelTimer();
                    StartLevelTimer();
                    break;
                case GameStates.LEVEL_COMPLETE:
                    StopLevelTimer();
                    break;
            }
        }
    }
}