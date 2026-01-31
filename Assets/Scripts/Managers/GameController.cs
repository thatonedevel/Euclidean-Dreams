using System;
using GameConstants.Enumerations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // game events
    public static event Action<GameStates> OnGameStateChanged;

    public GameStates CurrentGameState { get; private set; } = GameStates.PLAYING;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
}
