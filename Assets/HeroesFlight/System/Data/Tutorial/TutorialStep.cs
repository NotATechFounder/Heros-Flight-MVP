using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TutorialVisualData
{
    [SerializeField] private string title;
    [SerializeField] private List<TutorialStep> steps;
   
    public string Title => title;
    public List<TutorialStep> TutorialSteps => steps;
}

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 10)]
    [SerializeField] private string description;

    public string stepDescription => description;
}