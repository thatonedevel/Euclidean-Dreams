using EDreams;
using GameConstants;
using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.UIElements;
using Managers;
using Data;

namespace EGUI
{
    public class LevelCompleteUIController : MonoBehaviour
    {
        [Header("Object References")]
        [SerializeField] private UIDocument levelCompleteUI;
    
        // references to ui elements
        private Button nextStageButton;
        private Button replayStageButton;
        private Button stageSelectButton;
        private Label stageCompletionLabel;
        private Label stageClearTimeLabel;
    
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
            stageClearTimeLabel = levelCompleteUI.rootVisualElement.Query<Label>("ClearTimeLabel");
    
            // use this for the gauge references
    
            UQueryState<VisualElement> gaugeQuery = new UQueryBuilder<VisualElement>(levelCompleteUI.rootVisualElement)
                .Class("gaugeDisplay")
                .Build();
    
            // loop over this and store references in the gemGuages array
            gaugeQuery.ForEach((VisualElement gauge) => Util.TryAddItemToArray(gemGauges, gauge));
    
            // set up event subscriptions for buttons
            nextStageButton.clicked += () => GameController.Singleton.LoadGameLevel(GameController.Singleton.currentLevelNum + 1);
            replayStageButton.clicked += RestartLevelHandler;
            stageSelectButton.clicked += () => GameController.Singleton.GoToStageSelect();
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
                default:
                    // disable the ui
                    screenRoot.visible = false;
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
    
            // show clear time
            stageClearTimeLabel.text = string.Format("Clear Time: {0}", FormatClearTime(stageLevelData.levelClearTime));
    
            // set interactivity of the next level button based on if we're at the last level
            Debug.Log("Is at last level? " + GameController.Singleton.isAtLastLevel);
            nextStageButton.enabledSelf = !GameController.Singleton.isAtLastLevel;
    
            screenRoot.visible = true;
        }
    
        private string FormatClearTime(float time)
        {
            // format the given time in the format of mm:ss
            int minutes = (int)Mathf.Floor(time / 60);
            int remSeconds = (int)(time % 60);
    
            string minString = "00";
            string secString = "00";
    
            if (minutes.ToString().Length == 1)
                minString = "0" + minutes.ToString();
            else
                minString = minutes.ToString();
            
            if (remSeconds.ToString().Length == 1)
                secString = "0" + remSeconds.ToString();
            else
                secString = remSeconds.ToString();
    
            return minString + ":" + secString;
        }
    
        private void RestartLevelHandler()
        {
            screenRoot.visible = false;
    
            // also disable the gauges since this doesnt propagate properly?
    
            for (int i = 0; i < gemGauges.Length; i++)
            {
                gemGauges[i].visible = false;
            }
    
            GameController.Singleton.RestartLevel();
        }
    }
}

