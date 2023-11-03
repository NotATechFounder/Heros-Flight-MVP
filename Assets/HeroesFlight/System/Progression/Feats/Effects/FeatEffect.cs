using HeroesFlight.System.Stats.Feats.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Feats.Effects
{
    public class FeatEffect : ScriptableObject
    {
        [SerializeField] private FeatType featType;
        public FeatType FeatType => featType;
    }
}