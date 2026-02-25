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
        private List<VisualElement> levelPadlocks = new();
    
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

            UQueryState<VisualElement> lockQuery = new UQueryBuilder<VisualElement>(levelSelectDoc.rootVisualElement)
                .Class("level-padlock")
                .Build();
            
            // subscribe to the events / register the callbacks
            titleScreenButton.clicked += () => GameController.Singleton.StartGame();
            devGymButton.clicked += () => GameController.Singleton.LoadDevGym();
    
            buttonQuery.ForEach((Button btn) => { 
                levelSelectionButtons.Add(btn);
                btn.RegisterCallback<ClickEvent>(LevelSelectedCallback); 
            });
            
            lockQuery.ForEach((VisualElement padlock) => {
                levelPadlocks.Add(padlock);
            });
            
            // once this is done we can update the level locks. call here since this is in a dedicated scene
            UpdateLevelLocking();
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

        private void UpdateLevelLocking()
        {
            // use this method to update which levels are locked / unlocked
            int lastToUnlock = LevelProgressManager.Singleton.LastUnlockedStageIndex;

            for (int i = 0; i <= lastToUnlock; i++)
            {
                if (i > levelPadlocks.Count)
                    break;
                
                // set the stage to unlocked & enable the button
                levelPadlocks[i].visible = false;
                levelSelectionButtons[i].enabledSelf = true;
            }
        }
    }
}

