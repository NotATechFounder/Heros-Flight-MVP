using System;
using UnityEngine;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : ISystemInterface
    {

       event Action OnPlayerDeath;
       event Action OnPlayerWin;
       event Action<Transform, int> OnEnemyDamaged;
       event Action<int> OnCharacterHealthChanged; 
        void StartGameLoop();
    }
}