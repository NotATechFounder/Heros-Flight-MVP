using System;

namespace HeroesFlight.System.FileManager.Stats
{
    [Serializable]
    public class StatTypeWithValue
    {
        public StatType statType;
        public int value;
        public StatModel.StatCalculationType statCalculationType;

        public StatTypeWithValue(StatType statType, int value, StatModel.StatCalculationType statCalculationType)
        {
            this.statType = statType;
            this.value = value;
            this.statCalculationType = statCalculationType;
        }
    }
}