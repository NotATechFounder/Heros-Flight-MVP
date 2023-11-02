using System;
using HeroesFlight.System.Stats.Feats.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Feats
{
    [Serializable]
    public class FeatEffect
    {
        [SerializeField] private FeatType featType;
        public FeatType FeatType => featType;
    }
}