using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.InputSystem;
using GameConstants;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "TutorialTextSO", menuName = "Scriptable Objects/TutorialTextSO")]
    public class TutorialTextSO : ScriptableObject
    {
        public List<string> TutorialLines = new();
        public string currentLine = "";
        public int lineIndex { get; private set; } = 0;
    
        public static event Action OnTutorialFinished;

        // important actions so tutorial text reflects bindings
        private InputAction moveAction;
        private InputAction cameraZoomAction;
        private InputAction lookAction;
        private InputAction recenterCameraAction;
        private InputAction perspectiveSwitchAction;

        private InputAction[] actionArr;


        private void Awake()
        {
            if (TutorialLines.Count > 0)
            {
                currentLine = TutorialLines[0];
                lineIndex = 0;
            }
        }

        private void OnEnable()
        {
            // initialise the current line
            if (TutorialLines.Count > 0)
            {
                currentLine = TutorialLines[0];
                lineIndex = 0;
            }
            
            // grab the actions so we can show the bindings
            moveAction = InputSystem.actions.FindAction(Constants.ACTION_MOVE);
            cameraZoomAction = InputSystem.actions.FindAction(Constants.ACTION_ZOOM_CAMERA);
            lookAction = InputSystem.actions.FindAction(Constants.ACTION_ROTATE_CAMERA);
            recenterCameraAction = InputSystem.actions.FindAction(Constants.ACTION_CENTER_CAMERA);
            perspectiveSwitchAction = InputSystem.actions.FindAction(Constants.ACTION_SWITCH_PERSPECTIVE);
            actionArr = new[] {moveAction, lookAction, cameraZoomAction, perspectiveSwitchAction, perspectiveSwitchAction, recenterCameraAction};
        }
    
        public void AdvanceText()
        {
            if (lineIndex < TutorialLines.Count - 1)
            {
                lineIndex++;

                if (TutorialLines[lineIndex].Contains("{0}"))
                {
                    Debug.Log(lineIndex);
                    // we need to show the string binding here
                    string bindString = "";
                    var currentBinding = actionArr[lineIndex - 1].bindings[0];

                    if (currentBinding.isComposite)
                    {
                        // we need to join the composites
                        for (int i = 1; i < actionArr[lineIndex-1].bindings.Count; i++)
                        {
                            bindString = bindString + actionArr[lineIndex - 1].bindings[i].ToDisplayString();

                            if (i < actionArr[lineIndex - 1].bindings.Count - 1)
                            {
                                bindString = bindString + ", ";
                            }
                        }
                    }
                    else
                    {
                        bindString = currentBinding.ToDisplayString();
                    }
                    
                    currentLine = String.Format(TutorialLines[lineIndex], bindString);
                }
                else
                {
                    currentLine = TutorialLines[lineIndex];
                }
            }
            else
            {
                Debug.Log("finished the tutorial");
                // reached end of tutorial text
                OnTutorialFinished?.Invoke();
            }
        }
    }
}

