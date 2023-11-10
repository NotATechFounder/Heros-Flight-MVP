using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    public class HealthModificationIntentModel
    {
        public HealthModificationIntentModel(float damage,DamageType type, AttackType attackType,
            DamageCalculationType calculationType)
        {
            Amount = damage;
            DamageType = type;
            AttackType = attackType;
            CalculationType = calculationType;
        }
      
        public float Amount { get;  private set;}
        public DamageType DamageType { get; }
        public AttackType AttackType { get; }
        public DamageCalculationType CalculationType { get; }
        public IHealthController Target { get; }
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