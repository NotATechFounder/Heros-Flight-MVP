using UnityEngine;

namespace HeroesFlight.System.Stats.Traits.Effects
{
    [CreateAssetMenu(fileName = "RevivalEffect", menuName = "Traits/TraitEffects/RevivalEffect", order = 100)]
    public class RevivalEffect : TraitEffect
    {
        [SerializeField] private int revivalCount;
        [SerializeField] private float revivalHealthPercentage;


        public int RevivalCount => revivalCount;
        public float RevivalHealthPercentage => revivalHealthPercentage;
    }
}