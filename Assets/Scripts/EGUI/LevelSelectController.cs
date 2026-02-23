using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Managers;

namespace EGUI
{
    public class LevelSelectController : MonoBehaviour
    {
        [SerializeField] private UIDocument levelSelectDoc;
    
        private Button titleScreenButton;
        private Button devGymButton;
        private List<Button> levelSelectionButtons = new();
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // initialise ui
            titleScreenButton = levelSelectDoc.rootVisualElement.Query<Button>("TitleScreenButton");
            devGymButton = levelSelectDoc.rootVisualElement.Query<Button>("GymButton");
    
            // use a query builder to get all the level selection buttons
            UQueryState<Button> buttonQuery = new UQueryBuilder<Button>(levelSelectDoc.rootVisualElement)
                .Class("level-select-button")
                .Build();
    
            // subscribe to the events / register the callbacks
            titleScreenButton.clicked += () => GameController.Singleton.StartGame();
            devGymButton.clicked += () => GameController.Singleton.LoadDevGym();
    
            buttonQuery.ForEach((Button btn) => { 
                levelSelectionButtons.Add(btn);
                btn.RegisterCallback<ClickEvent>(LevelSelectedCallback); 
            });
        }
    
        private void LevelSelectedCallback(ClickEvent eventData)
        {
            // grab the button that was clicked
            int levelNum = -1;

            Button clickedButton = eventData.target as Button;
            
            // search the button list to find it
            levelNum =  levelSelectionButtons.IndexOf(clickedButton);
            GameController.Singleton.LoadGameLevel(levelNum);
        }
    }
}

