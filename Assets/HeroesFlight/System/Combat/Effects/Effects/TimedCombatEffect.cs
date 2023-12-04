using System;
using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [Serializable]
    public class TimedCombatEffect : CombatEffect
    {
        [SerializeField] protected EffectDurationType durationType;
        [SerializeField] protected float duration;
        public EffectDurationType DurationType => durationType;
        public float Duration => duration;
    }
}