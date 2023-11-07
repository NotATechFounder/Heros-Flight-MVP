using HeroesFlight.System.Stats.Traits.Enum;
using UnityEngine;


namespace HeroesFlight.System.Stats.Traits.Effects
{
    public class TraitEffect : ScriptableObject
    {
        [SerializeField] private TraitType traitType;
        [SerializeField] private int value;
        public TraitType TraitType => traitType;
        public int Value => value;
    }
}