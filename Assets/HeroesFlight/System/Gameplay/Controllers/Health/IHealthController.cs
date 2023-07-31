using System;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IHealthController
    {
        Transform currentTransform { get; }
        CombatTargetType TargetType { get; }
        int MaxHealth { get; }
        int CurrentHealth { get; }
        float CurrentHealthProportion { get; }
        void Init();
        event Action<Transform,int> OnBeingDamaged;
        event Action<IHealthController> OnDeath;
        void DealDamage(int damage);
        bool IsDead();
        
    }
}