using HeroesFlight.System.Stats.Feats.Effects;
using UnityEngine;

namespace HeroesFlight.System.Stats.Feats
{
    [CreateAssetMenu(fileName = "NewFeat", menuName = "Feats/Feat", order = 100)]
    public class Feat : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private Sprite image;
        [TextArea]
        [SerializeField] private string description;
        [SerializeField] private string blockingFeatId;
        [SerializeField] private FeatEffect effect;


        public string Id => id;
        public Sprite Icon => image;
        public string Description => description;
        public FeatEffect Effect => effect;
        public string DependantId => blockingFeatId;
    }
}