using GameConstants.Enumerations;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "TimerSO", menuName = "Scriptable Objects/TimerSO")]
    public class TimerSO : ScriptableObject
    {
        private const string TIME_FORMAT = "{0}:{1}";
        
        public string formattedLevelTime = "00:00";
    
        private float levelStartTime = 0;
        
        public float currentLevelTime = 0;
        public float currentGameTime = 0;

        public void StartTimer()
        {
            levelStartTime = Time.time;
        }

        public void FormatTime(bool forLevel = true)
        {
            string minString = "";
            string secString = "";
            
            // divide into min / sec component
            int minutes = (int)currentLevelTime / 60;
            int seconds = (int)currentLevelTime;

            if (currentLevelTime >= 60)
                seconds = (int)currentLevelTime % 60;

            if (minutes < 10)
                minString += "0" + minutes.ToString();
            else
                minString += minutes.ToString();
            
            if (seconds < 10)
                secString += "0" + seconds.ToString();
            else
                secString += seconds.ToString();
            
            if (forLevel) 
                formattedLevelTime = string.Format(TIME_FORMAT, minString, secString);
        }
    }
}

