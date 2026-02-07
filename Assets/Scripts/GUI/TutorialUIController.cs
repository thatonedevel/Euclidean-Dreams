using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TutorialUIController : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private UIDocument tutorialDoc;
    [SerializeField] private TutorialTextSO tutorialText;

    // inputs we need to listen for
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction perspectiveSwitchAction;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // initialise the input

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
