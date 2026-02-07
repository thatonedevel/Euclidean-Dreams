using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using GameConstants;

public class TutorialUIController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private UIDocument tutorialDoc;
    [SerializeField] private TutorialTextSO tutorialText;

    // inputs we need to listen for
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction cameraLookAction;
    private InputAction perspectiveSwitchAction;

    // events for the actions
    private PlayerInput.ActionEvent movedEvent;
    private PlayerInput.ActionEvent zoomEvent;
    private PlayerInput.ActionEvent lookEvent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // initialise the input
        moveAction = InputSystem.actions.FindAction(Constants.ACTION_MOVE);

        cameraLookAction = InputSystem.actions.FindAction(Constants.ACTION_ROTATE_CAMERA);
        zoomAction = InputSystem.actions.FindAction(Constants.ACTION_ZOOM_CAMERA);

        perspectiveSwitchAction = InputSystem.actions.FindAction(Constants.ACTION_SWITCH_PERSPECTIVE);

        // set up the events
        movedEvent = new PlayerInput.ActionEvent(moveAction);
        zoomEvent = new PlayerInput.ActionEvent(zoomAction);
        lookEvent = new PlayerInput.ActionEvent(cameraLookAction);

        // and add the callback
        movedEvent.AddListener(TutorialActionsListener);
        zoomEvent.AddListener(TutorialActionsListener);
        lookEvent.AddListener(TutorialActionsListener);
    }

    private void TutorialActionsListener(InputAction.CallbackContext context)
    {
        // just read the action & check which one it is
        if (context.action == moveAction)
        {
            if (tutorialText.lineIndex == 1)
                tutorialText.AdvanceText();
        }
        else if (context.action == zoomAction)
        {
             if (tutorialText.lineIndex == 3)
                tutorialText.AdvanceText();
        }
        else if (context.action == cameraLookAction)
        {
            if (tutorialText.lineIndex == 2)
                tutorialText.AdvanceText();
        }
    }
}
