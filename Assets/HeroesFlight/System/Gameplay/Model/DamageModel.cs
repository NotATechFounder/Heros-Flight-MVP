using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    public class DamageModel
    {
        public DamageModel(float damage,DamageType type,AttackType attackType)
        {
            Amount = damage;
            DamageType = type;
            AttackType = attackType;
        }
      
        public float Amount { get;  private set;}
        public DamageType DamageType { get; }
        public AttackType AttackType { get; }
        public Transform Target { get; private set; }

        public void SetTarget(Transform damageTarget)
        {
            Target = damageTarget;
        }

        public void ModifyAmount(float newValue)
        {
            Amount = newValue;
        }
    }
}