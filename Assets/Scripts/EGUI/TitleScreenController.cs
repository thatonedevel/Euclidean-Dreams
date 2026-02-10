using UnityEngine;
using UnityEngine.UIElements;
using Managers;

namespace EGUI
{
    public class TitleScreenCon : MonoBehaviour
    {
        [SerializeField] private UIDocument titleDoc;
    
        // button references
        private Button playButton;
        private Button settingsButton;
        private Button quitButton;
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // set references to buttons
            playButton = titleDoc.rootVisualElement.Query<Button>("PlayButton");
            settingsButton = titleDoc.rootVisualElement.Query<Button>("SettingsButton");
            quitButton = titleDoc.rootVisualElement.Query<Button>("QuitButton");
    
            // event subscription
            playButton.clicked += () => GameController.Singleton.GoToStageSelect();
            settingsButton.clicked += () => Debug.Log("Settings clicked"); // TODO: change this to open settings when implemented
            quitButton.clicked += () => Application.Quit();
        }
    }
}

