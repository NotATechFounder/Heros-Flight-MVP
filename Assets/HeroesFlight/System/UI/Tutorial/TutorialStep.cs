using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TutorialVisualData
{
    [SerializeField] private string title;
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    public string Title => title;
    public List<TutorialStep> TutorialSteps => tutorialSteps;
}

[System.Serializable]
public class TutorialStep
{
    public Sprite stepImage;
    [TextArea(3, 10)]
    public string stepDescription;
}
