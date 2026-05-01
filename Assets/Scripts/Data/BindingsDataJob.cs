using System;
using System.IO;
using System.Text;
using GameConstants;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Data
{
    public struct BindingsDataJob : IJob
    {
        public NativeArray<byte> persistentPathBytes;
        public NativeArray<byte> inputMapBytes;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Execute()
        {
            var mapString = Encoding.ASCII.GetString(inputMapBytes);
            // use this place to write the input action bindings to the disk
            // this is in a dedicated job as the map is kinda big
            try
            {
                File.WriteAllText(Encoding.ASCII.GetString(persistentPathBytes) + "/" + Constants.INPUT_MAP_NAME, mapString);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void CreateMapOfCurrentBindings(InputActionMap map)
        {
            // get json string of the map and store it in our native array
            inputMapBytes = new NativeArray<byte>(Encoding.ASCII.GetBytes(map.SaveBindingOverridesAsJson()), Allocator.TempJob);
        }
    }
}
