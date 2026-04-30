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
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void Execute()
        {
            var mapString = CreateMapOfCurrentBindings();
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

        private string CreateMapOfCurrentBindings()
        {
            var map = new InputActionMap();

            var defaultMap = InputSystem.actions.actionMaps[0];

            foreach (var action in defaultMap.actions)
            {
                map.AddAction(action.name, action.type, action.bindings[0].ToString());
            }

            return map.ToJson();
        }
    }
}
