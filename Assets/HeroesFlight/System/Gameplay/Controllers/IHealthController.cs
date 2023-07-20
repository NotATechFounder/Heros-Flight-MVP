using System;
using HeroesFlight.Common.Enum;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IHealthController
    {
        CombatTargetType TargetType { get; }
        int CurrentHealth { get; }
        void Init(int maxHealth);
        event Action OnBeingDamaged;
        bool IsDead();
        
    }
}