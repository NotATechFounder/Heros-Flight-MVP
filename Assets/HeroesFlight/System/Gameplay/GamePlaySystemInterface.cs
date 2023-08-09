using System;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : ISystemInterface
    {
        event Action<bool> OnMinibossSpawned;
        event Action<float> OnMinibossHealthChange; 
        event Action<int> OnRemainingEnemiesLeft;
        event Action<DamageModel> OnCharacterDamaged;

        event Action<DamageModel> OnEnemyDamaged;
        event Action<int> OnCharacterHealthChanged;
        public event Action<float, Transform> OnCharacterHeal;
        event Action<int> OnCharacterComboChanged; 
        event Action<GameState> OnGameStateChange;
        public event Action OnNextLvlLoadRequest;

        public event Action<BoosterSO, float, Transform> OnBoosterActivated;

        public event Action<int> OnCoinsCollected;

        public CurrencySpawner CurrencySpawner { get; }

        public CountDownTimer GameTimer { get; }
        public AngelEffectManager EffectManager { get; }

        public BoosterManager BoosterManager { get; }

        public BoosterSpawner BoosterSpawner { get; }
        public int CurrentLvlIndex { get; }
        void StartGameLoop(SpawnModel currentModel);
        void ContinueGameLoop(SpawnModel currentModel);
        void CreateCharacter();
        void ReviveCharacter();
        SpawnModel PreloadLvl();
        void ResetLogic();
        void EnablePortal();

        void AddGold (int amount);

        public void StoreRunReward();
    }
}