using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "CurrencyBoostEffect", menuName = "Traits/TraitEffects/CurrencyBoostEffect", order = 100)]
    public class CurrencyBoostEffect : TraitEffect
    {
        [SerializeField] private CurrencySO.CurrencyType targetType;
     
        public CurrencySO.CurrencyType CurrencyType => targetType;
       
    }
}