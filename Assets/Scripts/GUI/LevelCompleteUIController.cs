using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelCompleteUIController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private UIDocument levelCompleteUI;

    // references to ui elements
    private Button nextStageButton;
    private Button replayStageButton;
    private Button stageSelectButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to the game state change event 
        GameController.OnGameStateChanged += GameStateChangedHandler;

        // grab ui references
        nextStageButton = levelCompleteUI.rootVisualElement.Query<Button>("StageCompleteButton");
        replayStageButton = levelCompleteUI.rootVisualElement.Query<Button>("ReplayStageButton");
        stageSelectButton = levelCompleteUI.rootVisualElement.Query<Button>("StageSelectButton");

        // set up event subscriptions for buttons
        nextStageButton.clicked += NextStageHandler;
        replayStageButton.clicked += ReplayStageHandler;
        stageSelectButton.clicked += StageSelectHandler;
    }

    private void OnDestroy()
    {
        // unsubscribe from events here to prevent crashes
        GameController.OnGameStateChanged -= GameStateChangedHandler;
    }

    private void GameStateChangedHandler(GameStates newState, GameStates oldState)
    {
        switch (newState)
        {
            case GameStates.PLAYING:
                // we entered a level
                break;
            case GameStates.LEVEL_COMPLETE:
                ShowUI();
                break;
        }
    }

    private void ShowUI()
    {
        levelCompleteUI.enabled = true;
    }

    // handlers for the buttons

    private void NextStageHandler()
    {

    }

    private void StageSelectHandler()
    {

    }

    private void ReplayStageHandler()
    {

    }
}
