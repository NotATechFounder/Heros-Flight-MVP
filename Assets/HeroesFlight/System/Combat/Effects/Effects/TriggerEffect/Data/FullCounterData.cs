using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class FullCounterData : EffectData
    {
        public EffectValuePair Damage = new EffectValuePair();
    }
}