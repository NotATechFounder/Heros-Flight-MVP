using HeroesFlight.Common.Progression;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "AttributeEffect", menuName = "Traits/TraitEffects/AttributeEffect", order = 100)]
    public class StatBoostEffect : TraitEffect
    {
        [SerializeField] private HeroProgressionAttribute targetStat;
        [SerializeField] private int boostValue;
        public HeroProgressionAttribute TargetStat => targetStat;
        public int Value => boostValue;
    }
}