using HeroesFlight.System.Stats.Traits.Effects;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.Stats.Traits
{
    [CreateAssetMenu(fileName = "NewTrait", menuName = "Traits/Trait", order = 100)]
    public class Trait : ScriptableObject
    {
        [Header("Ui data")]
        [SerializeField] private string id;
        [SerializeField] private int requiredCharacterLvl;
        [SerializeField] private string blockingFeatId;
        [SerializeField] private Sprite image;
        [TextArea] 
        [SerializeField] private string description;
        [SerializeField] private int tier;
        [SerializeField] private int slot;
        [Header("Cost data")] 
        [SerializeField] private CurrencySO currency;
        [SerializeField] private int cost;
      
        [Header("Logical data")] 
        [SerializeField] private TraitEffect effect;


        public string Id => id;
        public Sprite Icon => image;
        public string Description => description;
        public TraitEffect Effect => effect;
        public string DependantId => blockingFeatId;
        public int RequiredLvl => requiredCharacterLvl;
        public int Tier => tier;
        public int Slot => slot;
        public int Cost => cost;
        public CurrencySO Currency => currency;


    }
}