using HeroesFlight.System.Stats.Traits.Enum;
using UnityEngine;


namespace HeroesFlight.System.Stats.Traits.Effects
{
    public class TraitEffect : ScriptableObject
    {
        [SerializeField] protected TraitType traitType;
        [SerializeField] private int value;
        [SerializeField] private bool canBeRerolled;
        [SerializeField] private Vector2Int valueRange;
        public TraitType TraitType => traitType;
        public int Value => value;
        public Vector2Int ValueRange => valueRange;
        public bool CanBeRerolled => canBeRerolled;
    }
}