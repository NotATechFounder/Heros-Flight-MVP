using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public abstract class StatusEffect : Effect
    {
        [SerializeField] protected EffectDurationType durationType;
        [SerializeField] protected float duration;
        [SerializeField] protected bool isStackable;
        [SerializeField] protected bool canBeReApplied;


        public EffectDurationType DurationType => durationType;
        public float Duration => duration;

        public bool IsStackable => isStackable;
        public bool CanReApply => canBeReApplied;
    }
}