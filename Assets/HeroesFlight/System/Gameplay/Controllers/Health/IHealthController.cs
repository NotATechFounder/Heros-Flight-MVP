using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IHealthController
    {
        Transform currentTransform { get; }
        CombatTargetType TargetType { get; }
        float MaxHealth { get; }
        float CurrentHealth { get; }
        float CurrentHealthProportion { get; }
        void Init();
        event Action<DamageModel> OnBeingDamaged;
        event Action<IHealthController> OnDeath;
        void DealDamage(DamageModel damage);
        bool IsDead();
        
    }
}