using HeroesFlight.System.Stats.Traits.Enum;
using UnityEngine;


namespace HeroesFlight.System.Stats.Traits.Effects
{
    public class TraitEffect : ScriptableObject
    {
        [SerializeField] private TraitType traitType;
        public TraitType TraitType => traitType;
    }
}