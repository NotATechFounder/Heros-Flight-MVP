using System;
using UnityEngine;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : ISystemInterface
    {
        event Action<int> OnRemainingEnemiesLeft; 
       event Action OnPlayerDeath;
       event Action OnPlayerWin;
       event Action<Transform, int> OnCharacterDamaged; 
       event Action<Transform, int> OnEnemyDamaged;
       event Action<int> OnCharacterHealthChanged; 
        void StartGameLoop();
    }
}