using GameConstants.Enumerations;
using Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace EGUI
{
    public class GameOverScreenController : MonoBehaviour
    {
        [SerializeField] private UIDocument gameOverDocument;
        private Button restartButton;
        private Button titleScreenButton;
        private VisualElement root;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            root = gameOverDocument.rootVisualElement.Q<VisualElement>("Root");
            restartButton = root.Q<Button>("RestartButton");
            titleScreenButton = root.Q<Button>("TitleScreenButton");

            GameController.OnGameStateChanged += GameUpdateListener;
        }

        private void GameUpdateListener(GameStates newState, GameStates oldState)
        {
            if (newState == GameStates.GAME_OVER)
            {
                // show self
                root.visible = true;
            }
        }

        private void RestartButtonClicked()
        {
            // hide self
            root.visible = false;
        }

        private void TitleScreenButtonClicked()
        {
            // hide self
            root.visible = false;
        }
    }
}
