using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.StatusEffects.Effects
{
    public class StatusEffectData
    {
        public StatusEffectType EffectType { get; }
        public StatusEffectApplyType ApplyType { get; }
        public StatusEffectDurationType DurationType { get; }
        public float Duration { get; }
        public float Value { get; }
        public GameObject Visual { get; }

        public StatusEffectData(StatusEffectType effectType, StatusEffectApplyType applyType,
            StatusEffectDurationType durationType, float duration, float value, GameObject visual)
        {
            EffectType = effectType;
            ApplyType = applyType;
            DurationType = durationType;
            Duration = duration;
            Value = value;
            Visual = visual;
        }
    }
}