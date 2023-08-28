using System;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;
using UnityEngine;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : SystemInterface
    {
        event Action<int> OnCountDownTimerUpdate;
        event Action<float> OnGameTimerUpdate;

        event Action<float> OnUltimateChargesChange; 
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
        event Action OnEnterMiniBossLvl;
        public event Action<BoosterSO, float, Transform> OnBoosterActivated;

        public event Action<int> OnCoinsCollected;
        public event Action<BoosterContainer> OnBoosterContainerCreated;
        public CurrencySpawner CurrencySpawner { get; }

        public CountDownTimer GameTimer { get; }
        public AngelEffectManager EffectManager { get; }
        public BoosterManager BoosterManager { get; }
        public BoosterSpawner BoosterSpawner { get; }
        public HeroProgression HeroProgression { get; }
        public GodsBenevolence GodsBenevolence { get; }
        public int CurrentLvlIndex { get; }
        public int MaxLvlIndex { get; }
        void StartGameLoop();
        void CreateCharacter();
        void ReviveCharacter();
        Level PreloadLvl();
        void ResetLogic();
        void EnablePortal();
        void UseCharacterSpecial();
        void AddGold (int amount);
        void AddExperience(int amount);
        void HandleSingleLevelUp();
        void HandleHeroProgression();
        public void StoreRunReward();
        void HeroProgressionCompleted();
    }
}