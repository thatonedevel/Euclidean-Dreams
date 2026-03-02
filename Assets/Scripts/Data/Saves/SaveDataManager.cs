using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using System.IO;
using System.Threading.Tasks;
using Managers;

namespace Data.Saves
{
    public class SaveDataManager : MonoBehaviour
    {
        // TODO: switch this over to use the .net serializer as unity's doesnt support 2d arrays
        
        const string SAVE_NAME = "player_data.json";
        
        [Header("Debug Information")]
        [SerializeField] private SaveSlotData saveData;

        private string saveDataJsonString = String.Empty;

        private Task<string> fileReadTask;
        
        // event fired once data is finished writing
        public static event Action<bool> SaveDataWriteComplete;
        public static event Action<int, int> SaveDataReadComplete;
        
        // class to manage current save data, including serialisation & i/o operations
        private void Start()
        {
            // check with the game controller how many stages exist
            int stageCount = GameController.Singleton.TotalLevelCount;
            
            print(Application.persistentDataPath);
            
            saveData = new SaveSlotData(stageCount);
            
            // check if the save file exists. if not, create it
            if (File.Exists(Application.persistentDataPath + "/ " + SAVE_NAME))
            {
                // check if the data is valid
                bool isValid = ReadSaveData();

                if (!isValid)
                    WriteSaveData(); // this will override it with a blank object
                
                // fire save data read complete slot
                SaveDataReadComplete?.Invoke(saveData.lastUnlockedMainStage, saveData.lastUnlockedBonusStage);
            }
            else
            {
                WriteSaveData();
            }
            
            // subscribe to the level progress update event here
            LevelProgressManager.LevelProgressUpdated += LevelCompletedListener;
        }

        private void WriteSaveData()
        {
            // serialize the stored save object to a json string
            saveDataJsonString = JsonUtility.ToJson(saveData);
            bool success = true;
            
            // use file write as it will be relatively small
            try
            {
                File.WriteAllTextAsync(Application.persistentDataPath + "/ " + SAVE_NAME, saveDataJsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                success = false;
            }
            
            // raise the event to say we're done
            SaveDataWriteComplete?.Invoke(success);
        }

        private bool ReadSaveData()
        {
            var text = File.ReadAllText(Application.persistentDataPath + "/ " + SAVE_NAME);

            SaveSlotData data = null;

            try
            {
                data = JsonUtility.FromJson<SaveSlotData>(text);
            }
            catch (Exception e)
            {
                saveData =  new SaveSlotData(GameController.Singleton.TotalLevelCount);
                return false;
            }

            if (data != null)
            {
                saveData = data;
                
                // check it contains the needed information. not unity type so we can use an is check
                if (!CheckInputDataIsValid())
                {
                    saveData = new SaveSlotData(GameController.Singleton.TotalLevelCount);
                    saveData.ConstructGemData();
                    saveData.FlattenGemData();
                    return false;
                }
                
                saveData.ConstructGemData();
                saveData.UnflattenGemData();
                return true;
            }

            Debug.LogWarning("Save data is null");
            saveData = new SaveSlotData(GameController.Singleton.TotalLevelCount);
            return false;
        }

        private void LevelCompletedListener(int lastStageIndex, int currenStageIndex, float completionTime, bool[] gemData)
        {
            // called once a level is completed.
            // retrieve the following data: time in stage, gems & level index
            saveData.lastUnlockedMainStage = lastStageIndex;
            saveData.savePlayTime +=  completionTime;
            saveData.gemCollectionStatus[currenStageIndex, 0] = gemData[0];
            saveData.gemCollectionStatus[currenStageIndex, 1] = gemData[1];
            saveData.gemCollectionStatus[currenStageIndex, 2] = gemData[2];
            
            saveData.FlattenGemData();
            
            WriteSaveData();
        }

        private bool CheckInputDataIsValid()
        {
            if (saveData.lastUnlockedMainStage < 0)
                return false;
            if (saveData.lastUnlockedBonusStage < -1)
                return false;
            if (saveData.gemCollectionStatus_flat == null)
                return false;
            if (saveData.gemCollectionStatus_flat.Length != GameController.Singleton.TotalLevelCount * 3)
                return  false;
            return true;
        }
    }
}
