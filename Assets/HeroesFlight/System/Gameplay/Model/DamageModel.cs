using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlight.System.Gameplay.Model
{
    public class DamageModel
    {
        public DamageModel(float damage,DamageType type)
        {
            Amount = damage;
            DamageType = type;
        }
      
        public float Amount { get; }
        public DamageType DamageType { get; }
        public Transform Target { get; private set; }

        public void SetTarget(Transform damageTarget)
        {
            Target = damageTarget;
        }
    }
}