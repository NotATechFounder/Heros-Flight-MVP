using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Container
{
    public class GameplayContainer : MonoBehaviour
    {
        [SerializeField] GameAreaModel currentModel;
        [SerializeField] LevelPortal portalPrefab;
        [SerializeField] BoosterDropSO mobDrop;
        [SerializeField] float heroProgressionExpEarnedPerKill = 20f;


        public event Action OnPlayerEnteredPortal;
        LevelPortal portal;
        public int CurrentLvlIndex { get; private set; }

        public bool FinishedLoop => CurrentLvlIndex >= currentModel.SpawnModel.Levels.Length;

        public int MaxLvlIndex => currentModel.SpawnModel.Levels.Length;

        public BoosterDropSO MobDrop => mobDrop;

        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;

        public void Init()
        {
            portal = Instantiate(portalPrefab, currentModel.PortalSpawnPosition, Quaternion.identity);
            portal.gameObject.SetActive(false);
            portal.OnPlayerEntered += HandlePlayerTriggerPortal;
        }

        public void SetStartingIndex(int startingIndex)
        {
            CurrentLvlIndex = startingIndex;
        }

        void HandlePlayerTriggerPortal()
        {
            OnPlayerEnteredPortal?.Invoke();
            portal.Disable();
        }

        public Level GetLevel()
        {
            if (CurrentLvlIndex >= currentModel.SpawnModel.Levels.Length)
                return null;

            Debug.LogError($"Returning model with index {CurrentLvlIndex}");
            Level level = currentModel.SpawnModel.Levels[CurrentLvlIndex];
            CurrentLvlIndex++;
            return level;
        }

        public void EnablePortal()
        {
            portal.Enable();
        }
    }
}