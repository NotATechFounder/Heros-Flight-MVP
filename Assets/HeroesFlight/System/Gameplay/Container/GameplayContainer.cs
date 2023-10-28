using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using HeroesFlightProject.System.Gameplay.Controllers.ShakeProfile;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Container
{
    public class GameplayContainer : MonoBehaviour
    {
        [SerializeField] GameAreaModel currentModel;
        [SerializeField] BoosterDropSO mobDrop;
        [SerializeField] ScreenShakeProfile bossProfile;

        public event Action OnPlayerEnteredPortal;
        LevelPortal portal;
        private Level currentLevel;

        public int CurrentLvlIndex { get; private set; }

        public ScreenShakeProfile BossProfile=>bossProfile;
        public bool FinishedLoop => CurrentLvlIndex >= currentModel.SpawnModel.Levels.Length;

        public int MaxLvlIndex => currentModel.SpawnModel.Levels.Length;

        public BoosterDropSO MobDrop => mobDrop;

        public float HeroProgressionExpEarnedPerKill => currentModel.HeroProgressionExpEarnedPerKill;

        public GameAreaModel CurrentModel => currentModel;

        public void Init()
        {
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

            if(currentLevel != null && currentLevel.LevelType != LevelType.Intermission && CurrentLvlIndex % 2 != 0)
            {
                return currentLevel = currentModel.AngelsGambitLevel;
            }

            currentLevel = currentModel.SpawnModel.Levels[CurrentLvlIndex];
            CurrentLvlIndex++;

            return currentLevel;
        }

        public Level GetAngelGambitLevel()
        {
            return currentModel.AngelsGambitLevel;
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