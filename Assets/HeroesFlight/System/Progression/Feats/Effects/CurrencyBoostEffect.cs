using UnityEngine;

namespace HeroesFlight.System.Stats.Feats.Effects
{
    [CreateAssetMenu(fileName = "CurrencyBoostEffect", menuName = "Feats/FeatEffects/CurrencyBoostEffect", order = 100)]
    public class CurrencyBoostEffect : FeatEffect
    {
        [SerializeField] private CurrencySO.CurrencyType targetType;
        [SerializeField] private float boostPercentageAmount;

        public CurrencySO.CurrencyType CurrencyType => targetType;
        public float BoostPercentageAmount => boostPercentageAmount;
    }
}