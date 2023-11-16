using System.Collections.Generic;
using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "CombatEffect", menuName = "Combat/Effects/CombatEffect", order = 100)]
    public class CombatEffect : ScriptableObject
    {
        [SerializeField] protected string id;
        [SerializeField] protected CombatEffectApplicationType applyType;
        [SerializeField] protected EffectDurationType durationType;
        [SerializeField] protected float duration;
        [SerializeField] protected GameObject visual;
        [SerializeField] private List<StatusEffect> statusEffects = new ();


        public CombatEffectApplicationType ApplyType => applyType;
        public EffectDurationType DurationType => durationType;
        public float Duration => duration;
        public GameObject Visual => visual;
        public string ID => id;
        public List<StatusEffect> StatusEffects => statusEffects;
    }
}