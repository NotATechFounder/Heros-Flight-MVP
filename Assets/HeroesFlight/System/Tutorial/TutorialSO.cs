using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSO", menuName = "Tutorial/TutorialSO", order = 1)]
public class TutorialSO : ScriptableObject
{
    [SerializeField] private TutorialMode tutorialState;
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    public List<TutorialStep> TutorialSteps => tutorialSteps;
    public TutorialMode TutorialState => tutorialState;
}
