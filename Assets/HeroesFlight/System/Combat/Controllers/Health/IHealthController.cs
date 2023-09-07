using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IHealthController
    {
        Transform HealthTransform { get; }
        bool IsImmortal { get; }
        float MaxHealth { get; }
        float CurrentHealth { get; }
        float CurrentHealthProportion { get; }
        void Init();
        event Action<DamageModel> OnBeingDamaged;
        event Action<IHealthController> OnDeath;
        event Action<float, Transform> OnHeal;
        event Action OnDodged;
        void DealDamage(DamageModel damage);
        bool IsDead();
        void Reset();
        void Revive();

        void SetInvulnerableState(bool isImmortal);

    }
}