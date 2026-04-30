using GameConstants;
using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace EGUI
{
    public class SettingsMenuController : MonoBehaviour
    {
        [Header("Objet References")] 
        [SerializeField] private UIDocument settingsDoc;

        [Header("Input Actions")] [SerializeField]
        private string[] inputActionKeys;

        private Dictionary<string, InputAction> actionDict = new();
        
        private List<Button> controlRemapButtons = new(); // buttons for remapping controls

        private VisualElement rootElement;
        
        // volume sliders
        private Slider masterVolSlider;
        private Slider musicVolSlider;
        private Slider sfxVolSlider;

        private const string CLASS_REMAP_BUTTON = "control-button";
        private const string ID_ROOT = "Root"; 

        private bool isListeningForBinding = false;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rootElement = settingsDoc.rootVisualElement.Q<VisualElement>(ID_ROOT);

            UQueryState<Button> btnQuery = new UQueryBuilder<Button>()
                .Class(CLASS_REMAP_BUTTON)
                .Build();
            
            controlRemapButtons = btnQuery.ToList();
            
            foreach (string actionKey in inputActionKeys)
            {
                InputAction tempAction = InputSystem.actions.FindAction(actionKey);
                if (tempAction is not null) actionDict.Add(actionKey, tempAction);
            }

            for (int i = 0; i < controlRemapButtons.Count; i++)
            {
                // use this to set the text of each button
                InputAction buttonAction = actionDict[inputActionKeys[i]];

                controlRemapButtons[i].text = buttonAction.activeControl.shortDisplayName;
            }
            
            // set up the dictionary of actions
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void OnRemapButtonClicked(ClickEvent evt)
        {
            var btn = evt.target as Button;
            
            // find the index of the button to determine the action we want to remap
        }

        private void RemapAction(InputAction targetAction)
        {
            // based off of the control remap example provided by Unity (2023), acc: 30/4/2026
            
        }

        private void CloseSettings() => rootElement.visible = false;
    }
}
