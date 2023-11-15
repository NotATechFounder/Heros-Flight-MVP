using System;
using HeroesFlight.Common.Progression;
using HeroesFlight.System.Stats.Traits.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "AttributeEffect", menuName = "Traits/TraitEffects/AttributeEffect", order = 100)]
    public class StatBoostEffect : TraitEffect
    {
        [SerializeField] private StatPointType targetStat;
        public StatPointType TargetStat => targetStat;

        private void Awake()
        {
            traitType = TraitType.StatBoost;
        }
    }
}