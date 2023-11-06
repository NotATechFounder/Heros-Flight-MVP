using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "CurrencyBoostEffect", menuName = "Traits/TraitEffects/CurrencyBoostEffect", order = 100)]
    public class CurrencyBoostEffect : TraitEffect
    {
        [SerializeField] private CurrencySO.CurrencyType targetType;
        [SerializeField] private float boostPercentageAmount;

        public CurrencySO.CurrencyType CurrencyType => targetType;
        public float BoostPercentageAmount => boostPercentageAmount;
    }
}