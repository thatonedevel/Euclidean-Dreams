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
        [SerializeField] private SaveSlotData activeSaveData;
        [SerializeField] private SaveSlotData[] saveDataSlots;
        [SerializeField] private int activeSlotIndex = 0;

        private string saveDataJsonString = String.Empty;

        private Task<string> fileReadTask;
        
        // event fired once data is finished writing
        public static event Action<bool> SaveDataWriteComplete;
        public static event Action<int, int> SaveDataReadComplete;
        
        public static SaveDataManager Singleton { get; private set; }
        
        // class to manage current save data, including serialisation & i/o operations

        private void Awake()
        {
            // set up singleton here
            if (Singleton != null)
                Destroy(gameObject);
            else
            {
                Singleton = this;
            }
        }
        
        
        private void Start()
        {
            // check with the game controller how many stages exist
            int stageCount = GameController.Singleton.TotalLevelCount;
            
            print(Application.persistentDataPath);
            
            activeSaveData = new SaveSlotData(stageCount);
            
            // check if the save file exists. if not, create it
            if (File.Exists(Application.persistentDataPath + "/ " + SAVE_NAME))
            {
                // check if the data is valid
                bool isValid = ReadSaveData();

                if (!isValid)
                    WriteSaveData(); // this will override it with a blank object
                
                // fire save data read complete slot
                SaveDataReadComplete?.Invoke(activeSaveData.lastUnlockedMainStage, activeSaveData.lastUnlockedBonusStage);
            }
            else
            {
                WriteSaveData();
            }
            
            // subscribe to the level progress update event here
            LevelProgressManager.LevelProgressUpdated += LevelCompletedListener;
        }

        private void WriteSaveData(string fileName = "")
        {
            // serialize the stored save object to a json string
            saveDataJsonString = JsonUtility.ToJson(activeSaveData);
            bool success = true;
            
            // use file write as it will be relatively small
            try
            {
                if (fileName.Equals(""))
                    File.WriteAllTextAsync(Application.persistentDataPath + "/" + SAVE_NAME, saveDataJsonString);
                else
                    File.WriteAllTextAsync(Application.persistentDataPath + "/" + fileName, saveDataJsonString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                success = false;
            }
            
            // raise the event to say we're done
            SaveDataWriteComplete?.Invoke(success);
        }

        private bool ReadSaveData(string fileName="")
        {
            string text = "";
            
            if (fileName.Equals(""))
                text = File.ReadAllText(Application.persistentDataPath + "/" + SAVE_NAME);
            else
                text = File.ReadAllText(Application.persistentDataPath + "/" + fileName);
            

            SaveSlotData data = null;

            try
            {
                data = JsonUtility.FromJson<SaveSlotData>(text);
            }
            catch (Exception e)
            {
                activeSaveData =  new SaveSlotData(GameController.Singleton.TotalLevelCount);
                return false;
            }

            if (data != null)
            {
                activeSaveData = data;
                
                // check it contains the needed information. not unity type so we can use an is check
                if (!CheckInputDataIsValid())
                {
                    activeSaveData = new SaveSlotData(GameController.Singleton.TotalLevelCount);
                    activeSaveData.ConstructGemData();
                    activeSaveData.FlattenGemData();
                    return false;
                }
                
                activeSaveData.ConstructGemData();
                activeSaveData.UnflattenGemData();
                return true;
            }

            Debug.LogWarning("Save data is null");
            activeSaveData = new SaveSlotData(GameController.Singleton.TotalLevelCount);
            return false;
        }

        private void LevelCompletedListener(int lastStageIndex, int currenStageIndex, float completionTime, bool[] gemData)
        {
            // called once a level is completed.
            // retrieve the following data: time in stage, gems & level index
            activeSaveData.lastUnlockedMainStage = lastStageIndex;
            activeSaveData.savePlayTime +=  completionTime;
            activeSaveData.gemCollectionStatus[currenStageIndex, 0] = gemData[0];
            activeSaveData.gemCollectionStatus[currenStageIndex, 1] = gemData[1];
            activeSaveData.gemCollectionStatus[currenStageIndex, 2] = gemData[2];
            
            activeSaveData.FlattenGemData();
            
            WriteSaveData();
        }

        private bool CheckInputDataIsValid()
        {
            if (activeSaveData.lastUnlockedMainStage < 0)
                return false;
            if (activeSaveData.lastUnlockedBonusStage < -1)
                return false;
            if (activeSaveData.gemCollectionStatus_flat == null)
                return false;
            if (activeSaveData.gemCollectionStatus_flat.Length != GameController.Singleton.TotalLevelCount * 3)
                return  false;
            return true;
        }

        private void ValidateAllSaves()
        {
            // check with the game controller how many stages exist
            int stageCount = GameController.Singleton.TotalLevelCount;
            
            //print(Application.persistentDataPath);
            
            // initialise all the saves
            for (int i = 0; i < saveDataSlots.Length; i++)
            {
                saveDataSlots[i] = new SaveSlotData(stageCount);
                // check if the save file exists. if not, create it
                if (File.Exists(Application.persistentDataPath + "/ " + SAVE_NAME))
                {
                    // check if the data is valid
                    bool isValid = ReadSaveData();
                    
                    if (!isValid)
                        WriteSaveData(); // this will override it with a blank object
                    
                    // fire save data read complete slot
                    SaveDataReadComplete?.Invoke(activeSaveData.lastUnlockedMainStage, activeSaveData.lastUnlockedBonusStage);
                }
                else
                {
                    WriteSaveData();
                }
            }
        }
        

        public bool[] GetGemStatusForStage(int stageIndex, bool isBonus=false)
        {
            // gets the gem status array for the specified stage
            // if the provided index is out of range, returns an empty array
            
            // TODO: add process for the bonus stages
            if (stageIndex >= 0 && stageIndex < GameController.Singleton.TotalLevelCount)
            {
                bool[] retArr = new bool[3];
                retArr[0] = activeSaveData.gemCollectionStatus[stageIndex, 0];
                retArr[1] = activeSaveData.gemCollectionStatus[stageIndex, 1];
                retArr[2] = activeSaveData.gemCollectionStatus[stageIndex, 2];

                return retArr;
            }
            else
            {
                return  new bool[] { };
            }
        }
    }
}
