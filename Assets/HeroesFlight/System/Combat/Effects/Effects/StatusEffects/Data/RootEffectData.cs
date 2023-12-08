using System;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class RootEffectData : EffectData
    {
        public EffectValuePair Damage = new EffectValuePair();
    }
}