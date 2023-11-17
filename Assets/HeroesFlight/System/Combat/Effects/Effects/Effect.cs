using HeroesFlight.System.Combat.StatusEffects.Enum;
using Pelumi.ObjectPool;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public class Effect : ScriptableObject
    {
        [SerializeField] protected EffectType effectType;
        [Range(0,100)]
        [SerializeField] protected float triggerChance;
        [SerializeField] protected float value;
        [SerializeField] protected Particle visual;
        public float Value => value;
        public float TriggerChance => triggerChance;
        public EffectType EffectType => effectType;
        public Particle Visual => visual;
    }
}