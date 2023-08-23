using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using UnityEngine;
using UnityEngine.Serialization;

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

        public bool FinishedLoop => CurrentLvlIndex >= currentModel.Models.Count;

        public int MaxLvlIndex => currentModel.Models.Count;

        public BoosterDropSO MobDrop => mobDrop;

        public float HeroProgressionExpEarnedPerKill => heroProgressionExpEarnedPerKill;

        public void Init()
        {
            portal = Instantiate(portalPrefab, currentModel.PortalSpawnPosition, Quaternion.identity);
            portal.OnPlayerEntered += HandlePlayerTriggerPortal;
            currentModel.Init();
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

        public SpawnModel GetCurrentLvlModel()
        {
            if (CurrentLvlIndex >= currentModel.Models.Count)
                return null;

            Debug.LogError($"Returning model with index {CurrentLvlIndex}");
            var model = currentModel.Models[CurrentLvlIndex];
            CurrentLvlIndex++;
            return model;
        }

        public void EnablePortal()
        {
            portal.Enable();
        }
    }
}