using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public abstract class StatusEffect : DynamicEffect
    {
        [SerializeField] protected EffectDurationType durationType;
        [SerializeField] protected float duration;
        [SerializeField] protected bool isStackable;
        [SerializeField] protected bool canBeReApplied;


        public EffectDurationType DurationType => durationType;
        public float Duration => duration;

        public bool IsStackable => isStackable;
        public bool CanReApply => canBeReApplied;

        public void SetData (StatusEffect statusEffect)
        {
            durationType = statusEffect.durationType;
            duration = statusEffect.duration;
            isStackable = statusEffect.isStackable;
            canBeReApplied = statusEffect.canBeReApplied;
            calculationType = statusEffect.calculationType;
            effectType = statusEffect.effectType;
            visual = statusEffect.visual;   
        }
    }
}