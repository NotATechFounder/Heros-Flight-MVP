using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "StatusEffect", menuName = "Combat/Effects/Status", order = 100)]
    public class StatusEffect : Effect
    {
        [SerializeField] protected EffectDurationType durationType;
        [SerializeField] protected float duration;
        [SerializeField] protected bool isStackable;
       
        
        public EffectDurationType DurationType => durationType;
        public float Duration => duration;
      
        public bool IsStackable => isStackable;
      
      
    }

    
}