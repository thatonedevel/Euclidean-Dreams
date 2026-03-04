using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace EGUI
{
    public class SaveSlotScreenController : MonoBehaviour
    {
        // class name constants
        private const string CLASS_PANEL = "save-panel";
        private const string CLASS_BUTTON_CONTAINER = "button-container";
        private const string CLASS_PLAY_BUTTON = "play-button";
        
        // lists for storing the widgets
        private List<VisualElement> slotPanels = new();
        private List<VisualElement> buttonContainers = new();
        
        // separate lists per type of button
        private List<Button> playSaveButtons = new();
        private List<Button> copySaveButtons = new();
        private List<Button> deleteSaveButtons = new();
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
