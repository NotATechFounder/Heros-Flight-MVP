using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class ReflectEffectData : EffectData
    {
        public EffectValuePair FlatDamage = new EffectValuePair();
        public EffectValuePair PercentageDamage = new EffectValuePair();
    }
}