using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Saves
{
    [Serializable]
    public class SaveSlotData
    {
        // struct to store all data for a session. includes last level index, gem collection states & playtime
        public int lastUnlockedMainStage = 0;
        public int lastUnlockedBonusStage = -1;

        public float savePlayTime = 0;
        
        public bool[,] gemCollectionStatus;

        public SaveSlotData(int levelCount)
        {
            // take the level count in constructor so we can properly size the gem coll status array
            gemCollectionStatus = new bool[levelCount, 3];
            
            for (int i = 0; i < gemCollectionStatus.Length; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    gemCollectionStatus[i, j] = false;
                }
            }
        }
    }
}
