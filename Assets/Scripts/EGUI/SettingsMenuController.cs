using System.IO;
using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Managers;
using Data.Jobs;
using Unity.Jobs;

namespace EGUI
{
    public class SettingsMenuController : MonoBehaviour
    {
        public static SettingsMenuController Singleton; // settings need to be accessed globally
        
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
        
        private Button closeSettingsButton;

        private const string CLASS_REMAP_BUTTON = "control-button";
        private const string ID_ROOT = "Root";
        private const string INPUT_BINDINGS = "input.json";
        
        private Button buttonToUpdate = null;

        private JobHandle readerHandle;
        private JobHandle keybindWriteHandle;
        private bool loadKeybindsComplete = false;
        private bool hasStartedLoading = false;
        private ReadJob keybindLoadJob;
        private WriteJob keybindWriter;
        private bool scheduledKeybindWrite = true;

        private void Awake()
        {
            if (Singleton != null)
                Destroy(gameObject);
            else
            {
                Singleton = this;
                readerHandle = new JobHandle();
                keybindWriteHandle = new JobHandle();
            }
        }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
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
            
            ReadActionMap();

            SetButtonBindingTexts();
            // register the callbacks for the buttons
            RegisterButtonCallbacks();
            
            closeSettingsButton = rootElement.Q<Button>("ExitSettingsButton");
            closeSettingsButton.clicked += CloseSettings;
            
            // sub to game state update
            GameController.OnGameStateChanged += GameStateUpdateListener;
        }

        private void LateUpdate()
        {
            if (keybindWriteHandle.IsCompleted && scheduledKeybindWrite)
            {
                keybindWriteHandle.Complete();
                scheduledKeybindWrite = false;
                keybindWriter.Free();
            }
        }

        private void OnDestroy()
        {
            GameController.OnGameStateChanged -= GameStateUpdateListener;
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

                if (currentAction is not null)
                {
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
        }

        private void RegisterButtonCallbacks()
        {
            for (int i = 0; i < controlRemapButtons.Count; i++)
            {
                controlRemapButtons[i].RegisterCallback<ClickEvent>(OnRemapButtonClicked);
            }
        }

        private void OnRemapButtonClicked(ClickEvent evt)
        {
            var btn = evt.target as Button;
            
            if (btn is null) return;

            buttonToUpdate = btn;

            var bindingDisplayName = buttonToUpdate.text;
            
            buttonToUpdate.text = "Waiting...";
            
            int btnIndex = controlRemapButtons.IndexOf(btn); // this will give us the key for the action
            
            // get binding and check if it is composite
            var rootBinding = actionDict[inputActionKeys[btnIndex]].bindings[0];

            if (!rootBinding.isComposite)
            {
                RemapAction(actionDict[inputActionKeys[btnIndex]]);
            }
            else
            {
                int bindIndex = GetPartialBindingIndex(inputActionKeys[btnIndex], bindingDisplayName);
                RemapAction(actionDict[inputActionKeys[btnIndex]], bindIndex);
            }
        }

        private int GetPartialBindingIndex(string actionKey, string bindingName)
        {
            string bindingPartName = actionKey.Split('/')[1];
            InputAction sourceAction = actionDict[actionKey];
            int bindingIndex = 1;

            for (int i = 1; i < sourceAction.bindings.Count; i++)
            {
                if (sourceAction.bindings[i].ToDisplayString().Equals(bindingName)) // HACK: should work but ideally we avoid string comp.
                {
                    bindingIndex = i;
                    break;
                }
            }
            return bindingIndex;
        }
        
        private void RemapAction(InputAction targetAction)
        {
            // based off of the control remap example provided by Unity (2023), acc: 30/4/2026
            // rebind system for non-composite actions
            
            // disable action before rebinding (voidsay, 2022)
            targetAction.Disable();
            
            targetAction.PerformInteractiveRebinding()
                .WithControlsExcluding("Mouse")
                .OnComplete(RebindComplete)
                .OnMatchWaitForAnother(0.1f)
                .Start();
        }

        private void RemapAction(InputAction targetAction, int bindingPartIndex)
        {
            // disable action before rebinding (voidsay, 2020)
            targetAction.Disable();
            
            // based on solution suggested by Daniel Sitarz (2022)
            targetAction.PerformInteractiveRebinding(bindingPartIndex)
                .WithControlsExcluding("Mouse")
                .OnComplete(RebindComplete)
                .OnMatchWaitForAnother(0.1f)
                .Start();
        }

        private void RebindComplete(InputActionRebindingExtensions.RebindingOperation op)
        {
            // re-enable action here (voidsay, 2020)
            op.action.Enable();
            
            // TODO: update button text with new input
            Debug.Log("Rebind Complete");

            SetButtonBindingTexts();
            buttonToUpdate = null; // release reference to this button
        }

        public void CloseSettings()
        {
            // start the write job to save changes
            WriteActionMap();
            rootElement.visible = false;
        }

        public void OpenSettings() => rootElement.visible = true;

        private void GameStateUpdateListener(GameStates newState, GameStates oldState)
        {
            if (newState == GameStates.PLAYING) 
                CloseSettings();
        }

        private void WriteActionMap()
        {

            keybindWriter = (WriteJob)IOJobFactory.CreateJob(
                Application.persistentDataPath + "/" + INPUT_BINDINGS,
                IOJobType.WRITE
                );
            keybindWriter.SetWriteBytes(actionDict[inputActionKeys[0]].actionMap.SaveBindingOverridesAsJson());
            keybindWriter.Schedule(keybindWriteHandle);
        }

        private void ReadActionMap()
        {
            // has to be ran on the main thread yay /s
            if (!File.Exists(Application.persistentDataPath + INPUT_BINDINGS))
            {
                File.CreateText(Application.persistentDataPath + "/" + INPUT_BINDINGS).Close();
            }

            var mapString = File.ReadAllText(Application.persistentDataPath + INPUT_BINDINGS);

            actionDict[inputActionKeys[0]].actionMap.LoadBindingOverridesFromJson(mapString);
        }
    }
}
