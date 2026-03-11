using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using System.Linq;
using Data.Saves;

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
        private const string CLASS_META_LABEL = "meta-label";
        
        // lists for storing the widgets
        private List<VisualElement> slotPanels = new();
        private List<VisualElement> buttonContainers = new();
        
        // separate lists per type of button
        private List<Button> playSaveButtons = new();
        private List<Button> copySaveButtons = new();
        private List<Button> deleteSaveButtons = new();
        
        // save metadata labels
        private List<Label> metadataLabels = new();
        
        // buttons not belonging to a group
        private Button titleScreenButton;
        private Button settingsButton;
        
        // flag for if we're selecting a target save for copy / deletion
        private bool isSelectingTargetSave = false;
        
        
        // hierarchy root we made
        private VisualElement customRoot;
        
        // list for the queries
        UQueryState<VisualElement>[] widgetQueries = new UQueryState<VisualElement>[5];
        
        [SerializeField] private UIDocument uiDocument;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            customRoot = uiDocument.rootVisualElement.Query<VisualElement>("Root");
            
            // queries for each of the "groups"
            CreateWidgetQueries();

            slotPanels = widgetQueries[0].ToList();
            buttonContainers = widgetQueries[1].ToList();
            CreateButtonArrays();
            SubscribeToButtonEvents();
            
            // register the hover callback
            foreach (var slot in slotPanels)
            {
                slot.RegisterCallback<MouseOverEvent>(SaveSlotHoverCallback);
                slot.RegisterCallback<MouseLeaveEvent>(SaveSlotExitCallback);
            }
            
            new UQueryBuilder<Label>(uiDocument.rootVisualElement)
                .Class(CLASS_META_LABEL)
                .Build()
                .ForEach(label => metadataLabels.Add(label));
            
            // update information on the metadata labels
            UpdateSaveStatus();
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
                widgetQueries[i] = new UQueryBuilder<VisualElement>(uiDocument.rootVisualElement)
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

        private void SubscribeToButtonEvents()
        {
            List<Button>[] buttonLists = {playSaveButtons, copySaveButtons, deleteSaveButtons};
            ButtonType[] buttonModes = { ButtonType.PLAY , ButtonType.COPY, ButtonType.DELETE };

            for (int typeIndex = 0; typeIndex < buttonModes.Length; typeIndex++)
            {
                for (int buttonIndex = 0; buttonIndex < buttonLists[typeIndex].Count; buttonIndex++)
                {
                    buttonLists[typeIndex][buttonIndex].clicked += MakeButtonAction(buttonIndex, buttonModes[typeIndex]);
                }
            }
        }

        private void SaveSlotHoverCallback(MouseOverEvent evt)
        {
            // check which of the slots we hovered over
            var hovered = evt.target as VisualElement;

            var ind = slotPanels.IndexOf(hovered);
            if (ind != -1)
            {
                buttonContainers[ind].visible = true;
            }
        }

        private void SaveSlotExitCallback(MouseLeaveEvent evt) // use this instead of MouseOutEvent, as pointed out by sebastiend-unity (2021)
        {
            // check which of the slots we hovered over
            var hovered = evt.target as VisualElement;

            var ind = slotPanels.IndexOf(hovered);
            if (ind != -1)
            {
                buttonContainers[ind].visible = false;
            }
        }

        private void UpdateSaveStatus()
        {
            // for each label, if a save exists, show the save info
            const string template = "Play Time: {0}\nSave Completion: {1}%";
            string outputTxt = "";
            
            for (int i = 0; i < metadataLabels.Count; i++)
            {
                // check if save is empty or not
                if (SaveDataManager.Singleton.IsSaveSlotEmpty(i))
                {
                    outputTxt = "Empty Save";
                    metadataLabels[i].text = outputTxt;
                    copySaveButtons[i].enabledSelf = false;
                    deleteSaveButtons[i].enabledSelf = false;
                }
                else
                {
                    float playTime = SaveDataManager.Singleton.GetSavePlayTime(i);
                    string timeString = FormatFloatTime(playTime);
                    int percent = SaveDataManager.Singleton.GetSaveCompletionPercentage(i);

                    outputTxt = string.Format(template, timeString, percent);
                    metadataLabels[i].text = outputTxt;
                    deleteSaveButtons[i].enabledSelf = true;
                    copySaveButtons[i].enabledSelf = true;
                }
            }
        }

        private string FormatFloatTime(float inputTime)
        {
            // takes an input seconds time & returns a string using mm:ss format
            int minutes = (int)inputTime / 60;
            int seconds = 0;
            if (minutes > 0)
            {
                seconds = (int)((float)inputTime % 60);
            }
            else
            {
                seconds = (int)inputTime;
            }
            
            // make sure we show the correct amount of digits
            var secString = seconds.ToString().Length == 1 ? "0" + seconds.ToString() : seconds.ToString();
            var minString = minutes.ToString().Length == 1 ? "0" + minutes.ToString() : minutes.ToString();
            
            return $"{minString}:{secString}";
        }

        // helper method to prevent late binding
        private Action MakeButtonAction(int buttonIndex, ButtonType buttonType)
        {
            Action outAction = null;
            
            switch (buttonType)
            {
                case ButtonType.PLAY:
                    outAction = () => PlayButtonPressed(buttonIndex);
                    break;
                case ButtonType.COPY:
                    outAction = () => CopyButtonPressed(buttonIndex);
                    break;
                case  ButtonType.DELETE:
                    outAction = () => DeleteButtonPressed(buttonIndex);
                    break;
            }

            return outAction;
        }
        
        // temp implementations to make sure references are set correctly
        private void PlayButtonPressed(int saveIndex)
        {
            // set the active save here & start the game
            SaveDataManager.Singleton.SetActiveSaveAndStart(saveIndex);
        }
        
        private void CopyButtonPressed(int saveIndex) => Debug.Log("CopyButtonPressed: " + saveIndex);
        private void DeleteButtonPressed(int saveIndex) => Debug.Log("DeleteButtonPressed: " + saveIndex);

        private enum ButtonType
        {
            PLAY,
            COPY,
            DELETE
        }
    }
}
