using System;
using GameConstants;
using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using LevelObjects;

namespace Managers
{
    public class GameController : MonoBehaviour
    {
        // game events
        public static event Action<GameStates, GameStates> OnGameStateChanged;
        
        // Class singleton
        public static GameController Singleton { get; private set; }
    
        public GameStates CurrentGameState { get; private set; } = GameStates.PLAYING;
    
        public bool isAtLastLevel { get; private set; } = false; // used to check if the current level is the last one
    
        public int currentLevelNum { get; private set; } = 0; // default to 0 as level numbers begin at 01
    
        [Header("Level object references")]
        [SerializeField] private GameObject playerCharacterObject;
        [SerializeField] private GameObject levelCameraRig;
    
        // lambda functions for events
        private void GoalReachedHandler() => UpdateGameState(GameStates.LEVEL_COMPLETE);
    
        // input specific to the game controller
        private InputAction pauseAction;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            // this object is going to be persistent, so we're safe to use a singleton pattern
            if (Singleton == null)
            {
                Singleton = this;
    
                Debug.Log("GameController: singleton initialised");
    
                // subscribe to important events
                LevelGoal.OnGoalReached += GoalReachedHandler;
                SceneManager.sceneLoaded += SceneChangedHandler;
            }
            else
            {
                DestroyImmediate(gameObject); // dispose of the gameobject & attached info to prevent duplicates
            }
        }
    
        private void Start()
        {
            // initialise input
            pauseAction = InputSystem.actions.FindAction(Constants.ACTION_PAUSE);
        }
    
        private void Update()
        {
            if (pauseAction.WasPressedThisFrame())
            {
                // toggle the pause menu
                if (CurrentGameState == GameStates.PLAYING)
                    PauseGame();
                else if (CurrentGameState == GameStates.PAUSED)
                    ResumeGame();
            }
        }
    
        private void UpdateGameState(GameStates newState)
        {
            // use this to adjust things when the game's state changes & fire the OnGameStateChanged event
            switch (newState)
            {
                case GameStates.PLAYING:
                    // enable char movement
                    playerCharacterObject.GetComponent<CharacterMovement>().enabled = true;
                    levelCameraRig.GetComponent<CameraControl>().enabled = true;
                    break;
                case GameStates.LEVEL_COMPLETE:
                    // disable char movement
                    playerCharacterObject.GetComponent<CharacterMovement>().enabled = false;
                    levelCameraRig.GetComponent<CameraControl>().enabled = false;
                    break;
                default:
                    // title screen, level select
                    currentLevelNum = 0;
                    isAtLastLevel = CheckLevelExistsAtIndex(currentLevelNum + 1);
    
                    // disable character movement if the character is not null
                    if (playerCharacterObject != null)
                    {
                        playerCharacterObject.GetComponent<CharacterMovement>().enabled = false;
                    }
                    break;
            }
    
            // invoke event sending the new state & state we transitioned from
            OnGameStateChanged?.Invoke(newState, CurrentGameState);
    
            // set new game state
            CurrentGameState = newState;
        }
    
        private void SceneChangedHandler(Scene newScene, LoadSceneMode mode)
        {
            // check the new scene's name to see if we're in a gameplay level
            if (newScene.name.StartsWith(Constants.LEVEL_PREFIX) || newScene.name.Equals("TestScene"))
            {
                Debug.Log("GameController: detected entry to level. updating references");
                // we went to a level
                // updare references to this level's cam & player object
                playerCharacterObject = GameObject.FindWithTag(Constants.TAG_PLAYER);
                levelCameraRig = GameObject.FindWithTag(Constants.TAG_CAMERA);
                // update game state to playing
                UpdateGameState(GameStates.PLAYING);
            }
        }
    
        public void RestartLevel()
        {
            // reload current level scene
            // check first it is an actual level
            if (SceneManager.GetSceneByBuildIndex(currentLevelNum).name.StartsWith(Constants.LEVEL_PREFIX))
            {
                // it is a level
                SceneManager.LoadSceneAsync(currentLevelNum);
            }
        }
    
        public void LoadGameLevel(int levelNumber)
        {
            // will use the levelNumber for the scene's buildIndex
            Debug.Log("Loading level: " + levelNumber);
            Scene levelScene = SceneManager.GetSceneByBuildIndex(levelNumber);
            Debug.Log("Level scene name: " + levelScene.name);
            SceneManager.LoadSceneAsync(levelNumber);
            currentLevelNum = levelNumber;
    
            // update the last level flag
            isAtLastLevel = !CheckLevelExistsAtIndex(levelNumber + 1);
        }

        public void LoadDevGym()
        {
            // method that loads the gym / dev room scene
            SceneManager.LoadSceneAsync("TestScene");
        }
    
        private void LevelCompletedHandler()
        {
            Debug.Log("Level completed");
        }
    
        public void StartGame()
        {
            // TODO: make this go to title screen
            // load first level
            SceneManager.LoadSceneAsync(Constants.SCENE_TITLE);
            UpdateGameState(GameStates.TITLE_SCREEN);
        }
    
        public void GoToStageSelect()
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_LEVEL_SELECT);
            UpdateGameState(GameStates.LEVEL_SELECT);
        }
    
        private bool CheckLevelExistsAtIndex(int sceneIndex)
        {
            // method to check if the supplied index value points to a game level
            Scene target = SceneManager.GetSceneByBuildIndex(sceneIndex);
    
            if (target.IsValid())
            {
                // scene exists. is it a level?
                return target.name.StartsWith(Constants.LEVEL_PREFIX);
            }
    
            return false;
        }
    
        public void PauseGame()
        {
            if (CurrentGameState == GameStates.PLAYING)
                UpdateGameState(GameStates.PAUSED);
        }
    
        public void ResumeGame()
        {
            if (CurrentGameState == GameStates.PAUSED)
                UpdateGameState(GameStates.PLAYING);
        }
    }
}

