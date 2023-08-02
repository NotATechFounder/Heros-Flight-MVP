using System;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Container
{
    public class GameplayContainer : MonoBehaviour
    {
        [SerializeField] GameLoopModel currentModel;
        [SerializeField] LevelPortal portalPrefab;
        public event Action OnPlayerEnteredPortal;
        LevelPortal portal;
        int index = 0;
        public bool FinishedLoop => index >= currentModel.Models.Count;

        public void Init()
        {
            portal = Instantiate(portalPrefab, currentModel.PortalSpawnPosition, Quaternion.identity);
            portal.OnPlayerEntered += HandlePlayerTriggerPortal;
        }

        public void SetStartingIndex(int startingIndex)
        {
            index = startingIndex;
        }
        void HandlePlayerTriggerPortal()
        {
            OnPlayerEnteredPortal?.Invoke();
            portal.Disable();
        }

        public SpawnModel GetCurrentLvlModel()
        {
            if (index >= currentModel.Models.Count)
                return null;
            
            Debug.LogError($"Returning model with index {index}");
            var model = currentModel.Models[index];
            index++;
            return model;
        }

        public void EnablePortal()
        {
            portal.Enable();
        }
    }
}