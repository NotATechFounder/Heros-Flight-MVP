using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialSO", menuName = "Tutorial/TutorialSO", order = 1)]
public class TutorialSO : ScriptableObject
{
    [SerializeField] private TutorialMode tutorialState;
    [SerializeField] private TutorialVisualData tutorialVisualData;
    public TutorialVisualData GetTutorialVisualData => tutorialVisualData;
    public TutorialMode tutorialMode => tutorialState;
}
