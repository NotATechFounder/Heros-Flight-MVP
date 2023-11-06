using System;
using HeroesFlight.System.Stats.Stats.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "AttributeEffect", menuName = "Traits/TraitEffects/AttributeEffect", order = 100)]
    public class StatBoostEffect : TraitEffect
    {
        [SerializeField] private StatType targetStat;
        [SerializeField] private int boostValue;
        public StatType TargetStat => targetStat;
        public int Value => boostValue;
    }
}