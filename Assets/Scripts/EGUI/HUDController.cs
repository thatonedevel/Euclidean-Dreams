using UnityEngine;
using UnityEngine.UIElements;
using Managers;
using System.Collections.Generic;
using GameConstants.Enumerations;
using LevelObjects;

namespace EGUI
{
   public class HUDController : MonoBehaviour
   {
       [Header("References")] [SerializeField]
       private UIDocument hudDoc;

       private VisualElement displayRoot;
       
       private Button pauseButton;
       private Button restartButton;

       private List<VisualElement> gemDisplays = new();
       
       // Start is called once before the first execution of Update after the MonoBehaviour is created
       void Start()
       {
           // set up ui
           displayRoot = hudDoc.rootVisualElement.Query<VisualElement>("Root");
           pauseButton = hudDoc.rootVisualElement.Query<Button>("PauseButton");
           restartButton = hudDoc.rootVisualElement.Query<Button>("RestartButton");

           UQueryState<VisualElement> gemQuery = new UQueryBuilder<VisualElement>()
               .Class("gem-display")
               .Build();

           gemQuery.ForEach((VisualElement element) => { gemDisplays.Add(element); });
           
           // subscribe to events
           GameController.OnGameStateChanged += GameStateChangedHandler;
           LevelObjects.Gem.OnGemCollected += GemCollectedHandler;
       }

       private void GemCollectedHandler(GemOrders order)
       {
           gemDisplays[(int)order].visible = true;
       }

       private void GameStateChangedHandler(GameStates newState, GameStates oldState)
       {
           switch (newState)
           {
               case GameStates.PLAYING:
                   if (oldState != GameStates.PAUSED)
                   {
                       ResetGemDisplays();
                   }
                   break;
           }
       }

       private void ResetGemDisplays()
       {
           foreach (var gem in gemDisplays)
           {
               gem.visible = false;
           }
       }
   }
}
