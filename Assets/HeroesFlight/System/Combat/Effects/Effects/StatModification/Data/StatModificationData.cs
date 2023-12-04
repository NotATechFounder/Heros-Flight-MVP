using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class StatModificationData 
    {
        public StatType TargetStat;
        public StatModel.StatCalculationType CalculationType;
        public EffectValuePair StatValue;
    }
}