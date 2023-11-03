using UnityEngine;

namespace HeroesFlight.System.Stats.Feats.Effects
{
    [CreateAssetMenu(fileName = "RevivalEffect", menuName = "Feats/FeatEffects/RevivalEffect", order = 100)]
    public class RevivalEffect : FeatEffect
    {
        [SerializeField] private int revivalCount;
        [SerializeField] private float revivalHealthPercentage;


        public int RevivalCount => revivalCount;
        public float RevivalHealthPercentage => revivalHealthPercentage;
    }
}