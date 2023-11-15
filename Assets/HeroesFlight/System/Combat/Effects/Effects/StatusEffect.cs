using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.StatusEffects.Effects
{
    public class StatusEffect : ScriptableObject
    {
        [SerializeField] protected StatusEffectType effectType;
        [SerializeField] protected StatusEffectDurationType durationType;
        [SerializeField] protected float duration;
        [SerializeField] protected float value;
        [SerializeField] protected GameObject visual;
    }
}