using System.Collections.Generic;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.StatusEffects.Effects
{
    public class CombatEffectData
    {
        public StatusEffectApplyType ApplyType { get; }
        public StatusEffectDurationType DurationType { get; }
        public float Duration { get; }
        public List<StatusEffect> Value { get; }
        public GameObject Visual { get; }

        public CombatEffectData( StatusEffectApplyType applyType,
            StatusEffectDurationType durationType, float duration, List<StatusEffect> value, GameObject visual)
        {
           ApplyType = applyType;
            DurationType = durationType;
            Duration = duration;
            Value = value;
            Visual = visual;
        }
    }
}