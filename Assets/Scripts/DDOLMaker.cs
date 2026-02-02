using UnityEngine;
using System.Collections.Generic;

public class DDOLMaker : MonoBehaviour
{
    [SerializeField] private List<GameObject> persistantObjects = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // loop through this list and mark them to be not destroyed when loading scenes
        for (int i = 0; i < persistantObjects.Count; i++)
        {
            DontDestroyOnLoad(persistantObjects[i]);
        }

        // call start game method to handle the game's new state
        GameController.Singleton.StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
