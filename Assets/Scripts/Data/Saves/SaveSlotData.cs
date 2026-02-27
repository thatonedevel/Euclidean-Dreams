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

        public bool[] gemCollectionStatus_flat;

        public SaveSlotData(int levelCount)
        {
            // take the level count in constructor so we can properly size the gem coll status array
            gemCollectionStatus = new bool[levelCount, 3];
            gemCollectionStatus_flat = new bool[levelCount * 3];

            int iter = 0;
            
            for (int i = 0; i < gemCollectionStatus.GetLength(0); i++)
            {
                for (int j = 0; j < gemCollectionStatus.GetLength(1); j++)
                {
                    gemCollectionStatus[i, j] = false;
                    gemCollectionStatus_flat[iter] = false;
                    if (iter + 1 < gemCollectionStatus_flat.Length)
                        iter++;
                }
            }
        }

        public void FlattenGemData()
        {
            // converts the data stored in the gemCollectionsStatus array to 1d data
            int flatIndex = 0;

            for (int i = 0; i < gemCollectionStatus.GetLength(0); i++)
            {
                for (int j = 0; j < gemCollectionStatus.GetLength(1); j++)
                {
                    gemCollectionStatus_flat[flatIndex] =  gemCollectionStatus[i, j];
                    flatIndex++;
                }
            }
            
            Debug.Log("FlattenGemData complete");
        }

        public void UnflattenGemData()
        {
            int flatIndex = 0;

            for (int i = 0; i < gemCollectionStatus.GetLength(0); i++)
            {
                for (int j = 0; j < gemCollectionStatus.GetLength(1); j++)
                {
                    gemCollectionStatus[i, j] = gemCollectionStatus_flat[flatIndex];
                    flatIndex++;
                }
            }
        }

        public void ConstructGemData()
        {
            int totalOuterLength = gemCollectionStatus_flat.Length / 3;
            
            gemCollectionStatus = new bool[totalOuterLength, 3];
        }
    }
}
