using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.StatusEffects.Effects
{
    public class StatusEffect : ScriptableObject
    {
        [SerializeField] protected StatusEffectType effectType;
        [SerializeField] protected StatusEffectApplyType applyType;
        [SerializeField] protected StatusEffectDurationType durationType;
        [SerializeField] protected float duration;
        [SerializeField] protected float value;
        [SerializeField] protected GameObject visual;

        public StatusEffectData GetData => new(effectType, applyType, durationType, duration, value, visual);
    }
}