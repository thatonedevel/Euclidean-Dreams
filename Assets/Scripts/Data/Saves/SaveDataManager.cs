using System;
using System.Text;
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
        public static event Action<string> SaveDataReadComplete;
        
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
                else
                    saveData = null; // for now discard it as we don't want this data yet
            }
            else
            {
                WriteSaveData();
            }
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
                return false;
            }

            if (data != null)
            {
                saveData = data;
                return true;
            }

            return false;
        }
    }
}
