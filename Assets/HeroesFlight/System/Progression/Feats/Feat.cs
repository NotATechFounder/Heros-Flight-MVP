using HeroesFlight.System.Stats.Feats.Effects;
using UnityEngine;

namespace HeroesFlight.System.Stats.Feats
{
    [CreateAssetMenu(fileName = "NewFeat", menuName = "Feats/Feat", order = 100)]
    public class Feat : ScriptableObject
    {
        [Header("Logical data")]
        [SerializeField] private string id;
        [SerializeField] private int requiredCharacterLvl;
        [SerializeField] private string blockingFeatId;
        [SerializeField] private FeatEffect effect;

        [Header("Ui data")]
        [SerializeField] private Sprite image;
        [TextArea]
        [SerializeField] private string description;


        public string Id => id;
        public Sprite Icon => image;
        public string Description => description;
        public FeatEffect Effect => effect;
        public string DependantId => blockingFeatId;
        public int RequiredLvl => requiredCharacterLvl;
    }
}