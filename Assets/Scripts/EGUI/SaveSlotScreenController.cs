using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using System.Linq;
using Unity.VisualScripting;

namespace EGUI
{
    public class SaveSlotScreenController : MonoBehaviour
    {
        // class name constants
        private const string CLASS_PANEL = "save-panel";
        private const string CLASS_BUTTON_CONTAINER = "button-container";
        private const string CLASS_PLAY_BUTTON = "play-button";
        private const string CLASS_COPY_BUTTON = "copy-button";
        private const string CLASS_DELETE_BUTTON = "delete-button";
        
        // lists for storing the widgets
        private List<VisualElement> slotPanels = new();
        private List<VisualElement> buttonContainers = new();
        
        // separate lists per type of button
        private List<Button> playSaveButtons = new();
        private List<Button> copySaveButtons = new();
        private List<Button> deleteSaveButtons = new();
        
        // buttons not belonging to a group
        private Button titleScreenButton;
        private Button settingsButton;
        
        // list for the queries
        UQueryState<VisualElement>[] widgetQueries = new UQueryState<VisualElement>[5];
        
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // queries for each of the "groups"
            CreateWidgetQueries();

            slotPanels = widgetQueries[0].ToList();
            buttonContainers = widgetQueries[1].ToList();
            CreateButtonArrays();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void CreateWidgetQueries()
        {
            string[] classes =
            {
                CLASS_PANEL,
                CLASS_BUTTON_CONTAINER,
                CLASS_PLAY_BUTTON,
                CLASS_COPY_BUTTON,
                CLASS_DELETE_BUTTON
            };

            for (int i = 0; i < classes.Length; i++)
            {
                widgetQueries[i] = new UQueryBuilder<VisualElement>()
                    .Class(classes[i])
                    .Build();
            }
        }

        private void CreateButtonArrays()
        {
            List<Button>[] buttonLists = {playSaveButtons, copySaveButtons, deleteSaveButtons};

            string[] buttonClasses = { CLASS_PLAY_BUTTON, CLASS_COPY_BUTTON, CLASS_DELETE_BUTTON};

            for (int i = 0; i < buttonLists.Length; i++)
            {
                for (int btnIndex = 0; btnIndex < widgetQueries[i + 2].Count(); btnIndex++)
                {
                    buttonLists[i].Add(widgetQueries[i+2].ElementAt(btnIndex) as Button);
                }
            }
        }
        
        
        // temp implementations to make sure references are set correctly
        private void PlayButtonPressed() => Debug.Log("PlayButtonPressed");
        private void CopyButtonPressed() => Debug.Log("CopyButtonPressed");
        private void DeleteButtonPressed() => Debug.Log("DeleteButtonPressed");
    }
}
