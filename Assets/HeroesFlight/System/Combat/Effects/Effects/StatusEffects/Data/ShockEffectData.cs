using System;


namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class ShockEffectData : EffectData
    {
        public EffectValuePair MainDamage = new EffectValuePair();
        public EffectValuePair SecondaryDamage = new EffectValuePair();
    }
}