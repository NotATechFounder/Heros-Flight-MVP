using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Controllers.Visuals;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface IHealthController : HitReceiverInterface
    {
        event Action<HealthModificationRequestModel> OnDamageReceiveRequest;
        event Action<IHealthController> OnDeath;
        event Action OnDodged;
        HealthType HealthType { get; }
        Transform HealthTransform { get; }
        bool IsImmortal { get; }
        float MaxHealth { get; }
        float CurrentHealth { get; }
        float CurrentHealthProportion { get; }
        float ImmortalityDuration { get; }
        void Init();
        void TryDealDamage(HealthModificationIntentModel healthModificationIntent);
        void ModifyHealth(HealthModificationIntentModel modificationIntentModel);
        void DealHealthPercentageDamage(float percentage, DamageCritType damageCritType,AttackType attackType);
        bool IsDead();
        void Revive(float healthPercentage);

        void SetInvulnerableState(bool isImmortal);

        // hit to move
        int MaxHit { get; }
        int CurrentHit { get; }
        public void DealHit();
        public event Action<Transform> OnBeingHitDamaged;
    }
}