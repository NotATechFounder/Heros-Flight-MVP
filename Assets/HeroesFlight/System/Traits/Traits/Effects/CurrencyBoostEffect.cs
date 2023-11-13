using System;
using HeroesFlight.System.Stats.Traits.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "CurrencyBoostEffect", menuName = "Traits/TraitEffects/CurrencyBoostEffect", order = 100)]
    public class CurrencyBoostEffect : TraitEffect
    {
        [SerializeField] private CurrencySO targetType;
     
        public CurrencySO CurrencyType => targetType;

        private void Awake()
        {
            traitType = TraitType.CurrencyBoost;
        }
    }
}