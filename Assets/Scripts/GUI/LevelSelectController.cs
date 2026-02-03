using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private UIDocument levelSelectDoc;

    private Button titleScreenButton;
    private List<Button> levelSelectionButtons = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialise ui
        titleScreenButton = levelSelectDoc.rootVisualElement.Query<Button>("TitleScreenButton");

        // use a query builder to get all the level selection buttons
        UQueryState<Button> buttonQuery = new UQueryBuilder<Button>(levelSelectDoc.rootVisualElement)
            .Class("level-select-button")
            .Build();

        // subscribe to the events / register the callbacks
        titleScreenButton.clicked += () => GameController.Singleton.StartGame();

        buttonQuery.ForEach((Button btn) => { 
            levelSelectionButtons.Add(btn);
            btn.RegisterCallback<ClickEvent>(LevelSelectedCallback); 
        });
    }

    private void LevelSelectedCallback(ClickEvent eventData)
    {
        // grab the button that was clicked
        int levelNum = -1;

        if (int.TryParse((eventData.target as Button).text, out levelNum))
        {
            // we clicked a button
            // tell the game manager to load the level based on the stored number
            Debug.Log("level selector: Level to load: " + levelNum);
            GameController.Singleton.LoadGameLevel(levelNum);
        }
    }
}
