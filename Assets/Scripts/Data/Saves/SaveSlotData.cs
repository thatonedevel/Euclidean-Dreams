using UnityEngine;
using UnityEngine.Serialization;

namespace Data.Saves
{
    public struct SaveSlotData
    {
        // struct to store all data for a session. includes last level index, gem collection states & playtime
        public int lastUnlockedMainStage;
        public int lastUnlockedBonusStage;

        public float savePlayTime;
        
        public bool[,] gemCollectionStatus;
    }
}
