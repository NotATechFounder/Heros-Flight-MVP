using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDataHolder : MonoBehaviour
{
    [SerializeField] TutorialHand tutorialHand;  
    [SerializeField] TutorialSO[] tutorialSOs;

    private Dictionary<TutorialMode, TutorialSO> tutorialDictionary = new Dictionary<TutorialMode, TutorialSO>();

    public TutorialHand GetTutorialHand => tutorialHand;

    public void Init()
    {
        for (int i = 0; i < tutorialSOs.Length; i++)
        {
            tutorialDictionary.Add(tutorialSOs[i].tutorialMode, tutorialSOs[i]);
        }
    }

    public TutorialSO GetTutorialSO(TutorialMode tutorialMode)
    {
        return tutorialDictionary[tutorialMode];
    }
}

public class TutorialRuntime
{
    public TutorialMode TutorialMode;
    public bool IsCompleted;
    public Action OnBegin;
    public Action OnEnd;

    public TutorialRuntime(TutorialMode tutorialMode)
    {
        TutorialMode = tutorialMode;
    }

    public void AssignEvents(Action onBegin, Action onEnd)
    {
        OnBegin = onBegin;
        OnEnd = onEnd;
    }
}