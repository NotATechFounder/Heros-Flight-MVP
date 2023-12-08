using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public abstract class DynamicEffect : Effect
    {
        [SerializeField] protected CalculationType calculationType;

        [SerializeField] protected EffectType effectType;
        [SerializeField] protected GameObject visual;
        public CalculationType CalculationType => calculationType;
        public EffectType EffectType => effectType;
        public GameObject Visual => visual;
    }
}