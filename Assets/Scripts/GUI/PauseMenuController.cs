using UnityEngine;
using UnityEngine.UIElements;
using GameConstants.Enumerations;

public class PauseMenuController : MonoBehaviour
{
    // allow singleton pattern for this class as we will only ever have a single pause screen
    public static PauseMenuController Singleton { get; private set; }

    [Header("Reference to GUI Document")]
    [SerializeField] private UIDocument pauseUiDocument;

    // references to ui elements
    private VisualElement pauseMenuRoot;

    private Button resumeButton;
    private Button restartButton;
    private Button settingsButton;
    private Button stageSelectButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // initialise ui references
        pauseMenuRoot = pauseUiDocument.rootVisualElement.Query<VisualElement>("Root");

        resumeButton = pauseUiDocument.rootVisualElement.Query<Button>("ResumeButton");
        restartButton = pauseUiDocument.rootVisualElement.Query<Button>("RestartButton");
        settingsButton = pauseUiDocument.rootVisualElement.Query<Button>("SettingsButton");
        stageSelectButton = pauseUiDocument.rootVisualElement.Query<Button>("StageSelectButton");

        // subscribe to important events
        GameController.OnGameStateChanged += GameStateUpdateHandler;

        // button event subscription
        resumeButton.clicked += ResumeButtonPressed;
        restartButton.clicked += RestartButtonPressed;
        settingsButton.clicked += SettingsButtonPressed;
        stageSelectButton.clicked += StageSelectButtonPressed;
    }

    private void OnDestroy()
    {
        // unsubscribe from all events
        GameController.OnGameStateChanged -= GameStateUpdateHandler;
        resumeButton.clicked -= ResumeButtonPressed;
        restartButton.clicked -= RestartButtonPressed;
        settingsButton.clicked -= SettingsButtonPressed;
        stageSelectButton.clicked -= StageSelectButtonPressed;
    }

    private void OpenPauseMenu()
    {
        pauseMenuRoot.visible = true;
    }
    public void ClosePauseMenu()
    {
        pauseMenuRoot.visible = false;
    }

    private void GameStateUpdateHandler(GameStates newState, GameStates oldState)
    {
        if (newState == GameStates.PAUSED && oldState == GameStates.PLAYING)
        {
            OpenPauseMenu();
        }
        else
        {
            ClosePauseMenu();
        }
    }

    // methods for button presses
    private void ResumeButtonPressed()
    {
        GameController.Singleton.ResumeGame();
    }

    private void RestartButtonPressed()
    {
        GameController.Singleton.RestartLevel();
    }

    private void SettingsButtonPressed()
    {
        // TODO: call to open settings menu once implemented
    }

    private void StageSelectButtonPressed()
    {
        GameController.Singleton.GoToStageSelect();
    }
}
