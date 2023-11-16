using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    public class HealthModificationIntentModel
    {
        public HealthModificationIntentModel(float damage,DamageCritType critType, AttackType attackType,
            DamageCalculationType calculationType,IHealthController damageSource)
        {
            Amount = damage;
            DamageCritType = critType;
            AttackType = attackType;
            CalculationType = calculationType;
            Source = damageSource;
        }
      
        public float Amount { get;  private set;}
        public DamageCritType DamageCritType { get; }
        public AttackType AttackType { get; }
        public DamageCalculationType CalculationType { get; }
        public IHealthController Target { get; }
        public IHealthController Source { get; }
        public Transform TargetTransform { get; private set; }

        public void SetTarget(Transform damageTarget)
        {
            TargetTransform = damageTarget;
        }

        public void ModifyAmount(float newValue)
        {
            Amount = newValue;
        }
    }
}