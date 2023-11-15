using System.Collections.Generic;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.Combat.StatusEffects.Effects
{
    public class CombatEffect : ScriptableObject
    {
        
        [SerializeField] protected StatusEffectApplyType applyType;
        [SerializeField] protected StatusEffectDurationType durationType;
        [SerializeField] protected float duration;
  
        [SerializeField] protected GameObject visual;
        [SerializeField] private List<StatusEffect> statusEffects = new ();
       

        public CombatEffectData GetData => new( applyType, durationType, duration, statusEffects, visual);
    }
}