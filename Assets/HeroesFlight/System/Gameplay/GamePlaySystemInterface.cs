using System;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Model;

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
        event Action<int> OnCharacterComboChanged; 
        event Action<GameState> OnGameStateChange;
        public event Action OnNextLvlLoadRequest;
        public CountDownTimer GameTimer { get; }
        public AngelEffectManager EffectManager { get; }

        public BoosterManager BoosterManager { get; }

        public BoosterSpawner BoosterSpawner { get; }
        public int CurrentLvlIndex { get; }
        void StartGameLoop(SpawnModel currentModel);
        void ContinueGameLoop(SpawnModel currentModel);
        void ReviveCharacter();
        SpawnModel PreloadLvl();
        void ResetLogic();
        void EnablePortal();
    }
}