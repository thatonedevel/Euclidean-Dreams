using GameConstants.Enumerations;
using Managers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "TimerSO", menuName = "Scriptable Objects/TimerSO")]
    public class TimerSO : ScriptableObject
    {
        public string formattedTime = "{0}:{1}";
    
        private float levelStartTime = 0;

        public void StartTimer()
        {
            levelStartTime = Time.time;
        }

        public void FormatTime()
        {
            string minString = "";
            string secString = "";
            
            // get current time
            float timeElapased = Time.time - levelStartTime;
            
            // divide into min / sec component
            int minutes = (int)timeElapased / 60;
            int seconds = (int)timeElapased % 60;

            if (minutes < 10)
                minString += "0" + minutes.ToString();
            else
                minString += minutes.ToString();
            
            if (seconds < 10)
                secString += "0" + seconds.ToString();
            else
                secString += seconds.ToString();
            
            formattedTime = string.Format(formattedTime, minutes, seconds);
        }
    }
}

