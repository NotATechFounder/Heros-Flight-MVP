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
    [SerializeField] TutorialTrigger tutorialTrigger;
    [SerializeField] Reward firstItemReward;

    [Header("Settings")]
    [SerializeField] float timeStopRestoreSpeed = 10f;
    [SerializeField] float timeStopDuration = 0.02f;

    private LevelPortal portal;
    private Level currentLevel;

    public int CurrentLvlIndex { get; private set; }
    public bool FinishedLoop => CurrentLvlIndex >= tutorialModel.SpawnModel.Levels.Length;
    public int MaxLvlIndex => tutorialModel.SpawnModel.Levels.Length;
    public GameAreaModel GetTutorialModel => tutorialModel;
    public BoosterDropSO GetMobDrop => mobDrop;
    public TutorialTrigger GetTutorialTrigger => tutorialTrigger;
    public Level GetCurrentLevel => currentLevel;
    public Reward FirstItemReward => firstItemReward;
    public float TimeStopRestoreSpeed => timeStopRestoreSpeed;
    public float TimeStopDuration => timeStopDuration;


    public void Init()
    {
        portal = Instantiate(tutorialModel.PortalPrefab, transform.position, Quaternion.identity);
        portal.gameObject.SetActive(false);
        portal.OnPlayerEntered += HandlePlayerTriggerPortal;
    }

    private void HandlePlayerTriggerPortal()
    {
        OnPlayerEnteredPortal?.Invoke();
    }

    public Level GetLevel()
    {
        if (currentLevel != null)
        {
            return currentLevel = tutorialModel.ShrineLevel;
        }

        currentLevel = tutorialModel.SpawnModel.Levels[CurrentLvlIndex];
        CurrentLvlIndex++;

        return currentLevel;
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
}