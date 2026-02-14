using GameConstants.Enumerations;
using UnityEngine;
using ScriptableObjects;

namespace Managers
{
    public class GameTimeManager : MonoBehaviour
    {
        // class for managing play / level time
        // TODO: implement full play time with the save system

        private float TotalPlayTime = 0;
        [SerializeField] private float currentLevelTime = 0;
        
        public static GameTimeManager Singleton { get; private set; }
        
        private bool updateGameTime = false;
        private bool updateLevelTime = false;
        
        // start times so we can calculate the total time elapsed
        private float gameStartTime = 0;
        private float levelStartTime = 0;
        
        [Header("References")]
        [SerializeField] private TimerSO timer;
        
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

            if (updateLevelTime)
            {
                currentLevelTime = Time.time - levelStartTime;
                // write the time data to the timer so & format it
                timer.currentLevelTime = currentLevelTime;
                //print("formatting time");
                timer.FormatTime();
            }
        }

        public void ResetAllTimers()
        {
            // resets all timers & stops them
            updateGameTime = false;
            updateLevelTime = false;
            
            TotalPlayTime = 0;
            currentLevelTime = 0;
        }


        public void ResetLevelTimer()
        {
            updateLevelTime = false;
            currentLevelTime = 0;
        }
        
        public void StartLevelTimer()
        {
            levelStartTime = Time.time;
            updateLevelTime = true;
        }

        public void StopLevelTimer()
        {
            updateLevelTime = false;
        }

        public void StopGameTimer()
        {
            
        }

        private void GameStateUpdateHandler(GameStates newState, GameStates oldState)
        {
            switch (newState)
            {
                case GameStates.PLAYING:
                    if (oldState != GameStates.PLAYING && oldState != GameStates.PAUSED)
                    {
                        print("starting level timer");
                        ResetLevelTimer();
                        StartLevelTimer();
                    }
                    break;
                case GameStates.LEVEL_COMPLETE:
                    StopLevelTimer();
                    break;
            }
        }
    }
}