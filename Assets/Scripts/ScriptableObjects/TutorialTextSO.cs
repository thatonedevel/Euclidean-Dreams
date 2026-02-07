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
            currentLine = TutorialLines[lineIndex];
        }
    }

    private void OnEnable()
    {
        // initialise the current line
        if (TutorialLines.Count > 0)
            currentLine = TutorialLines[lineIndex];
    }

    public void AdvanceText()
    {
        if (lineIndex < TutorialLines.Count)
        {
            lineIndex++;
            currentLine = TutorialLines[lineIndex];
        }
        else
        {
            // reached end of tutorial text
            OnTutorialFinished?.Invoke();
        }
    }
}
