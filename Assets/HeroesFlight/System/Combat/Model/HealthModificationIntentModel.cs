using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    public class HealthModificationIntentModel
    {
        public HealthModificationIntentModel(float damage, DamageCritType critType, AttackType attackType,
            CalculationType calculationType, IHealthController damageAttacker, int damageInstances = 1, float delayBetweenInstances= 0.2f)
           
        {
            Amount = damage;
            DamageCritType = critType;
            AttackType = attackType;
            CalculationType = calculationType;
            Attacker = damageAttacker;
            DamageInstancesCount = damageInstances;
            DelayBetweenInstances = delayBetweenInstances;
        }
      
        public float Amount { get;  private set;}
        public DamageCritType DamageCritType { get; }
        public AttackType AttackType { get; }
        public CalculationType CalculationType { get; }
        public IHealthController Attacker { get; }
        public Transform DefenderTransform { get; private set; }
        public int DamageInstancesCount { get; private set; }
        public float DelayBetweenInstances { get; private set; }

        public void SetTarget(Transform damageTarget)
        {
            DefenderTransform = damageTarget;
        }

        public void ModifyAmount(float newValue)
        {
            Amount = newValue;
        }
    }
}