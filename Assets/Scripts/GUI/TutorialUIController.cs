using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using GameConstants;
using GameConstants.Enumerations;
using System.Collections;

public class TutorialUIController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private UIDocument tutorialDoc;
    [SerializeField] private TutorialTextSO tutorialText;

    [Header("Tutorial Settings")]
    [SerializeField] private float introDuration;

    // inputs we need to listen for
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction cameraLookAction;
    private InputAction perspectiveSwitchAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // initialise the input
        moveAction = InputSystem.actions.FindAction(Constants.ACTION_MOVE);

        cameraLookAction = InputSystem.actions.FindAction(Constants.ACTION_ROTATE_CAMERA);
        zoomAction = InputSystem.actions.FindAction(Constants.ACTION_ZOOM_CAMERA);

        perspectiveSwitchAction = InputSystem.actions.FindAction(Constants.ACTION_SWITCH_PERSPECTIVE);


        // subscribe to the performed event
        moveAction.performed += TutorialActionsListener;
        zoomAction.performed += TutorialActionsListener;
        cameraLookAction.performed += TutorialActionsListener;

        PerspectiveSwitcher.OnDimensionsSwitched += FirstDimSwitchHandler;
        TutorialTextSO.OnTutorialFinished += TutorialFinishedHandler;

        StartCoroutine(routine: FirstSectionCoroutine());
    }

    private void OnDestroy()
    {
        PerspectiveSwitcher.OnDimensionsSwitched -= FirstDimSwitchHandler;
        moveAction.performed -= TutorialActionsListener;
        zoomAction.performed -= TutorialActionsListener;
        cameraLookAction.performed -= TutorialActionsListener;
        TutorialTextSO.OnTutorialFinished -= TutorialFinishedHandler;
    }

    private void TutorialActionsListener(InputAction.CallbackContext context)
    {
        Debug.Log("Action detected");

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

    private void FirstDimSwitchHandler(Dimensions newDim)
    {
        // check the current index
        if (tutorialText.lineIndex == 4 || tutorialText.lineIndex == 5)
        {
            tutorialText.AdvanceText();
        }
    }

    private void TutorialFinishedHandler()
    {
        tutorialDoc.rootVisualElement.visible = false;
    }

    private IEnumerator FirstSectionCoroutine()
    {
        Debug.Log("Starting wait");
        yield return new WaitForSeconds(introDuration);
        Debug.Log("Finished wait");
        tutorialText.AdvanceText();
    }
}
