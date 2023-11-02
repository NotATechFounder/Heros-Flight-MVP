using System;
using HeroesFlight.System.Stats.Stats.Enum;
using UnityEngine;

namespace HeroesFlight.System.Stats.Feats
{
    [Serializable]
    public class StatBoostEffect : FeatEffect
    {
        [SerializeField] private StatType targetStat;
        [SerializeField] private int boostValue;
        public StatType TargetStat => targetStat;
        public int Value => boostValue;
    }
}