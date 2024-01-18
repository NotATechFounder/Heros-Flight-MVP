using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.Gameplay.Controllers.ShakeProfile;
using UnityEditor;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Container
{
    public class GameplayContainer : MonoBehaviour
    {
        [SerializeField] GameAreaModel[] gameAreaModels;

        [Header("Settings")]
        [SerializeField] float timeStopRestoreSpeed = 10f;
        [SerializeField] float timeStopDuration = 0.02f;
        [SerializeField] ScreenShakeProfile bossProfile;

        public event Action OnPlayerEnteredPortal;
        LevelPortal portal;
        private Level currentLevel;
        private GameAreaModel currentModel;

        public float TimeStopRestoreSpeed => timeStopRestoreSpeed;
        public float TimeStopDuration => timeStopDuration;
        public int CurrentLvlIndex { get; private set; }
        public ScreenShakeProfile BossProfile=>bossProfile;
        public bool FinishedLoop => CurrentLvlIndex >= currentModel.SpawnModel.Levels.Length;
        public int MaxLvlIndex => currentModel.SpawnModel.Levels.Length;
        public float HeroProgressionExpEarnedPerKill => currentModel.HeroProgressionExpEarnedPerKill;
        public GameAreaModel CurrentModel => currentModel;

        public void Init(WorldType worldType)
        {
            currentModel = Array.Find(gameAreaModels, x => x.WorldType == worldType);
            portal = Instantiate(currentModel.PortalPrefab, transform.position, Quaternion.identity);
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
        }

        public Level GetLevel()
        {
            if (CurrentLvlIndex >= currentModel.SpawnModel.Levels.Length)
                return null;

            if(currentLevel != null && currentLevel.LevelType != LevelType.Shrine && CurrentLvlIndex % 3 == 0)
            {
                return currentLevel = currentModel.ShrineLevel;
            }

            currentLevel = currentModel.SpawnModel.Levels[CurrentLvlIndex];
            CurrentLvlIndex++;

            return currentLevel;
        }

        public float GetRunXpForLevel(LevelType levelType)
        {
            foreach (var entry in currentModel.InRunXp.inRunXpEntries)
            {
                if (entry.LevelType == levelType)
                    return entry.xp;
            }
            return 0;
        }

        public void EnablePortal(Vector2 position)
        {
            portal.Enable(position);
        }

        public void DisablePortal()
        {
            portal.Disable();
        }
    }
}