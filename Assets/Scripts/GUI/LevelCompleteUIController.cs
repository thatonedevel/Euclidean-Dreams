using EDreams;
using GameConstants;
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
    private Label stageCompletionLabel;

    // gem gauges
    private VisualElement[] gemGauges = new VisualElement[3];

    // reference to the stage's leveldata instance
    LevelData stageLevelData;

    // hierarchy root element as shown in the builder
    private VisualElement screenRoot;

    // composite string for the level name
    const string lvCompleteText = "Stage\n{0}\ncomplete";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to the game state change event 
        GameController.OnGameStateChanged += GameStateChangedHandler;

        // grab ui references
        nextStageButton = levelCompleteUI.rootVisualElement.Query<Button>("NextStageButton");
        replayStageButton = levelCompleteUI.rootVisualElement.Query<Button>("ReplayStageButton");
        stageSelectButton = levelCompleteUI.rootVisualElement.Query<Button>("StageSelectButton");
        screenRoot = levelCompleteUI.rootVisualElement.Query<VisualElement>("UIRoot");
        stageCompletionLabel = levelCompleteUI.rootVisualElement.Query<Label>("StageCompleteLabel");

        // use this for the gauge references

        UQueryState<VisualElement> gaugeQuery = new UQueryBuilder<VisualElement>(levelCompleteUI.rootVisualElement)
            .Class("gaugeDisplay")
            .Build();

        // loop over this and store references in the gemGuages array
        gaugeQuery.ForEach((VisualElement gauge) => Util.TryAddItemToArray(gemGauges, gauge));

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
        Debug.Log("detected state change");
        switch (newState)
        {
            case GameStates.PLAYING:
                // we entered a level
                break;
            case GameStates.LEVEL_COMPLETE:
                Debug.Log("UI Controller: showing UI");
                ShowUI();
                break;
        }
    }

    private void ShowUI()
    {
        // update level data reference
        stageLevelData = GameObject.FindGameObjectWithTag(Constants.TAG_LEVEL_DATA).GetComponent<LevelData>();

        // set gem guage visibility
        for (int i = 0; i < gemGauges.Length; i++) 
        {
            gemGauges[i].visible = stageLevelData.gemCollectionStatus[i];
        }

        // set stage name text
        stageCompletionLabel.text = string.Format(lvCompleteText, stageLevelData.GetStageName());

        screenRoot.visible = true;
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
