using System;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects.Data
{
    [Serializable]
    public class FreezeEffectData : EffectData
    {
        public EffectValuePair SlowAmount = new EffectValuePair();
    }
}