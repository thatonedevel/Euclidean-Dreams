using GameConstants;
using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;

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
        private int rebindButtonIndex = -1;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            int controlBindingIndex = 1;
            rootElement = settingsDoc.rootVisualElement.Q<VisualElement>(ID_ROOT);

            UQueryState<Button> btnQuery = new UQueryBuilder<Button>(rootElement)
                .Class(CLASS_REMAP_BUTTON)
                .Build();
            
            controlRemapButtons = btnQuery.ToList();
            
            foreach (string actionKey in inputActionKeys)
            {
                string mainActionKey = actionKey.Split('/')[0]; // for composite actions, main action header is before a /
                
                InputAction tempAction = InputSystem.actions.FindAction(mainActionKey);
                if (tempAction is not null) actionDict.Add(actionKey, tempAction);
            }

            SetButtonBindingTexts();
        }

        private void SetButtonBindingTexts()
        {
            InputAction currentAction = null;
            int compositeBindingIndex = 1;
            
            for (int i = 0; i < controlRemapButtons.Count; i++)
            {
                // use this to set the text of each button
                InputAction tempAction = actionDict[inputActionKeys[i]];
                
                // check if the action has changed
                if (currentAction == tempAction)
                {
                    // action has not changed, increment comp. index
                    compositeBindingIndex++;
                }
                else
                {
                    // new action, reset index
                    compositeBindingIndex = 1;
                    currentAction = tempAction;
                }
                
                // work out which binding we need to get
                var mainBinding = currentAction.bindings[0];
                var sourceBinding = mainBinding;
                
                if (mainBinding.isComposite)
                {
                    // get the partial binding
                    sourceBinding = currentAction.bindings[compositeBindingIndex];
                }

                controlRemapButtons[i].text = sourceBinding.ToDisplayString();
            }
        }

        private void OnRemapButtonClicked(ClickEvent evt)
        {
            var btn = evt.target as Button;
            
            // find the index of the button to determine the action we want to remap
        }

        private void RemapAction(InputAction targetAction)
        {
            // based off of the control remap example provided by Unity (2023), acc: 30/4/2026
            // rebind system for non-composite actions

            targetAction.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(RebindComplete)
                .Start();
        }

        private void RebindComplete(InputActionRebindingExtensions.RebindingOperation op)
        {
            // TODO: update button text with new input
            Debug.Log("Rebind Complete");
        }
        
        private void CloseSettings() => rootElement.visible = false;
    }
}
