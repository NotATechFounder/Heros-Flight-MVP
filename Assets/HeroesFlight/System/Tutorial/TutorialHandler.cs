using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.Gameplay.Controllers.ShakeProfile;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHandler : MonoBehaviour
{
    public event Action OnPlayerEnteredPortal;

    [SerializeField] GameAreaModel tutorialModel;
    [SerializeField] BoosterDropSO mobDrop;
    [SerializeField] TutorialSO[] gameplayTutorial;

    private int currentTutorialIndex;
    private LevelPortal portal;
    private Level currentLevel;

    public int CurrentLvlIndex { get; private set; }
    public bool FinishedLoop => CurrentLvlIndex >= tutorialModel.SpawnModel.Levels.Length;
    public int MaxLvlIndex => tutorialModel.SpawnModel.Levels.Length;
    public GameAreaModel TutorialModel => tutorialModel;
    public BoosterDropSO MobDrop => mobDrop;
    public Level CurrentLevel => currentLevel;

    public TutorialSO[] GameplayTutorial => gameplayTutorial;

    private Dictionary<TutorialMode, TutorialSO> tutorialDictionary = new Dictionary<TutorialMode, TutorialSO>();

    public void Init()
    {
        portal = Instantiate(tutorialModel.PortalPrefab, transform.position, Quaternion.identity);
        portal.gameObject.SetActive(false);
        portal.OnPlayerEntered += HandlePlayerTriggerPortal;

        for (int i = 0; i < gameplayTutorial.Length; i++)
        {
            tutorialDictionary.Add(gameplayTutorial[i].tutorialMode, gameplayTutorial[i]);
        }
    }

    private void HandlePlayerTriggerPortal()
    {
        OnPlayerEnteredPortal?.Invoke();
    }

    public Level GetLevel()
    {
        if (CurrentLvlIndex >= tutorialModel.SpawnModel.Levels.Length)
            return null;

        if (CurrentLvlIndex == 0)
        {
            return currentLevel = tutorialModel.SpawnModel.Levels[CurrentLvlIndex];
        }

        if (tutorialModel != null && currentLevel.LevelType != LevelType.Shrine && CurrentLvlIndex % 2 != 0)
        {
            return currentLevel = tutorialModel.AngelsGambitLevel;
        }

        currentLevel = tutorialModel.SpawnModel.Levels[CurrentLvlIndex];
        CurrentLvlIndex++;

        return currentLevel;
    }

    public Level GetAngelGambitLevel()
    {
        return tutorialModel.AngelsGambitLevel;
    }

    public void EnablePortal(Vector2 position)
    {
        portal.Enable(position);
    }

    public void DisablePortal()
    {
        portal.Disable();
    }

    internal void SetStartingIndex(int v)
    {
        CurrentLvlIndex = v;
    }

    public TutorialSO LoadTutorial()
    {
        return gameplayTutorial[currentTutorialIndex];
    }

    public void StartTutorialState(Action OnBegin, Func<bool> condition,  Action OnEnd)
    {
        StartCoroutine(TutorialState(OnBegin, condition,  OnEnd));
    }

    public IEnumerator TutorialState(Action onBegin, Func<bool> condition, Action onEnd)
    {
        onBegin?.Invoke();
        while (!condition())
        {
            yield return null;
        }
        onEnd?.Invoke();
    }

    public TutorialSO TutorialComplete(TutorialMode tutorialMode)
    {
        if (gameplayTutorial[currentTutorialIndex].tutorialMode == tutorialMode)
        {
            currentTutorialIndex++;
        }
        return LoadTutorial ();
    }

    internal TutorialSO GetTutorialSO(TutorialMode fly)
    {
        return tutorialDictionary[fly];
    }
}
