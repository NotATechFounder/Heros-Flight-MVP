using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeroesFlight.System.FileManager;

public class TutorialDataHolder : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        public bool IsCompleted;
    }

    public const string SAVE_ID = "TutorialData";

    [SerializeField] TutorialHand tutorialHand;  
    [SerializeField] TutorialSO[] tutorialSOs;
    private Data data = new Data();

    private Dictionary<TutorialMode, TutorialSO> tutorialDictionary = new Dictionary<TutorialMode, TutorialSO>();

    public TutorialHand GetTutorialHand => tutorialHand;

    public Data GetData => data;

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

    public void TutorialCompleted()
    {
        data.IsCompleted = true;
        Save();
    }

    public void Load()
    {
        Data savedData = FileManager.Load<Data>(SAVE_ID);
        data = savedData ?? new Data();
    }

    public void Save()
    {
        FileManager.Save(SAVE_ID, data);
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