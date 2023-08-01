using System;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;

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
        event Action<GameplayState> OnGameStateChange;
        public CountDownTimer GameTimer { get; }
        void StartGameLoop();
        void ReviveCharacter();
    }
}