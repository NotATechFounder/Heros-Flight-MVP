using System;
using System.Collections.Generic;
using HeroesFlight.System.Combat.Effects.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [Serializable]
    public class CombatEffect
    {
        [SerializeField] protected string id;
        [SerializeField] protected CombatEffectApplicationType applyType;
        [SerializeField] protected GameObject visual;
        [SerializeField] private List<Effect> effectToApply = new();


        public CombatEffectApplicationType ApplyType => applyType;
       
        public GameObject Visual => visual;
        public string ID => id;
        public List<Effect> EffectToApply => effectToApply;
    }
}