using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "StatusEffect", menuName = "Combat/Effects/Status", order = 100)]
    public class StatusEffect : ScriptableObject
    {
        [SerializeField] protected StatusEffectType effectType;
        [SerializeField] protected EffectDurationType durationType;
        [SerializeField] protected float triggerChance;
        [SerializeField] protected float duration;
        [SerializeField] protected float value;
        [SerializeField] protected bool isStackable;
        [SerializeField] protected GameObject visual;
        
        
        public StatusEffectType EffectType => effectType;
        public EffectDurationType DurationType => durationType;
        public float Duration => duration;
        public float Value => value;
        public bool IsStackable => isStackable;
        public float TriggerChance => triggerChance;
        public GameObject Visual => visual;
      
    }

    
}