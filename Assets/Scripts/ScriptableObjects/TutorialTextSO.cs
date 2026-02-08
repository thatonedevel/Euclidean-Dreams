using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "TutorialTextSO", menuName = "Scriptable Objects/TutorialTextSO")]
public class TutorialTextSO : ScriptableObject
{
    public List<string> TutorialLines = new();
    public string currentLine = "";
    public int lineIndex { get; private set; } = 0;

    public static event Action OnTutorialFinished;


    private void Awake()
    {
        if (TutorialLines.Count > 0) 
        {
            currentLine = TutorialLines[0];
            lineIndex = 0;
        }
    }

    private void OnEnable()
    {
        // initialise the current line
        if (TutorialLines.Count > 0)
        {
            currentLine = TutorialLines[0];
            lineIndex = 0;
        } 
    }

    public void AdvanceText()
    {
        if (lineIndex < TutorialLines.Count - 1)
        {
            lineIndex++;
            currentLine = TutorialLines[lineIndex];
        }
        else
        {
            Debug.Log("finished the tutorial");
            // reached end of tutorial text
            OnTutorialFinished?.Invoke();
        }
    }
}
