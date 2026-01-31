using System;
using GameConstants;
using GameConstants.Enumerations;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    // game events
    public static event Action<GameStates> OnGameStateChanged;
    
    // Class singleton
    public static GameController Singleton { get; private set; }

    public GameStates CurrentGameState { get; private set; } = GameStates.PLAYING;

    private int currentLevelNum = 0; // default to 0 as level numbers begin at 01

    [Header("Level object references")]
    [SerializeField] private GameObject playerCharacterObject;
    [SerializeField] private GameObject levelCameraRig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // this object is going to be persistent, so we're safe to use a singleton pattern
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            DestroyImmediate(gameObject); // dispose of the gameobject & attached info to prevent duplicates
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateGameState(GameStates newState)
    {
        // use this to adjust things when the game's state changes & fire the OnGameStateChanged event
        switch (newState)
        {

        }
    }

    private void SceneChangedHandler(Scene newScene, LoadSceneMode mode)
    {
        // check the new scene's name to see if we're in a gameplay level
        if (newScene.name.StartsWith(Constants.LEVEL_PREFIX))
        {
            // we went to a level
            // update game state to playing
            UpdateGameState(GameStates.PLAYING);
            // update the reference to the player character to use this levels' instance

            playerCharacterObject = GameObject.FindWithTag(Constants.TAG_PLAYER);
            levelCameraRig = GameObject.FindWithTag(Constants.TAG_CAMERA);
        }
    }

    private void RestartLevel()
    {

    }

    private void LoadGameLevel(int levelNumber)
    {
        // will use the levelNumber for the scene's buildIndex
        Debug.Log("Loading level: " + levelNumber);
        Scene levelScene = SceneManager.GetSceneByBuildIndex(levelNumber);
        Debug.Log("Level scene name: " + levelScene.name);
        SceneManager.LoadSceneAsync(levelNumber);
    }

    private void LevelCompletedHandler()
    {
        Debug.Log("Level completed");
    }
}
