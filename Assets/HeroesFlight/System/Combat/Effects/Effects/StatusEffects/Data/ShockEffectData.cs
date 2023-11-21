using System;


namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class ShockEffectData : EffectData
    {
        public EffectValuePair MainDamage;
        public EffectValuePair SecondaryDamage;
    }
}