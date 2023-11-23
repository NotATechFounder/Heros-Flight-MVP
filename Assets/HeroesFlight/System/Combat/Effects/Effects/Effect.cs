using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public abstract class Effect : ScriptableObject
    {
        [SerializeField] protected EffectType effectType;
        [SerializeField] protected CalculationType calculationType;
        [SerializeField] protected GameObject visual;
        public EffectType EffectType => effectType;
        public GameObject Visual => visual;
        public CalculationType CalculationType => calculationType;

        public abstract  T GetData<T>() where T : class;

    }
}