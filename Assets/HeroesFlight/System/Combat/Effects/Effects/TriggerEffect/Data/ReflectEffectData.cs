using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class ReflectEffectData : EffectData
    {
        public EffectValuePair FlatDamage;
        public EffectValuePair PercentageDamage;
    }
}