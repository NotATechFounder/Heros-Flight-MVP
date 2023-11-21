using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class SacrificeEffectData : EffectData
    {
        public EffectValuePair DamageBoost;
        public EffectValuePair HealthThreshhold;
    }
}