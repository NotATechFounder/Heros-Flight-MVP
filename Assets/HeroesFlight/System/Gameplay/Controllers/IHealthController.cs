using System;
using HeroesFlight.Common.Enum;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IHealthController
    {
        CombatTargetType TargetType { get; }
        int CurrentHealth { get; }
        void Init();
        event Action OnBeingDamaged;
        event Action OnDeath;
        void DealDamage(int damage);
        bool IsDead();
        
    }
}