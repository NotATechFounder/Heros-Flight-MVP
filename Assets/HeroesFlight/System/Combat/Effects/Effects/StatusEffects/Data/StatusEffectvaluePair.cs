using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class StatusEffectValuePair
    {
        public float StartValue;
        public float IncreasePerLvl;

        public float GetCurrentValue(int lvl)
        {
            return StartValue + IncreasePerLvl * (lvl-1);
        }
    }
}