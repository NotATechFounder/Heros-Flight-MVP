using System;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : ISystemInterface
    {
        event Action<bool> OnMinibossSpawned;
        event Action<float> OnMinibossHealthChange; 
        event Action<int> OnRemainingEnemiesLeft;
        event Action<Transform, float> OnCharacterDamaged;
        event Action<Transform, float> OnEnemyDamaged;
        event Action<int> OnCharacterHealthChanged;
        event Action<int> OnCharacterComboChanged; 
        event Action<GameplayState> OnGameStateChange;
        public CountDownTimer GameTimer { get; }
        void StartGameLoop();
    }
}