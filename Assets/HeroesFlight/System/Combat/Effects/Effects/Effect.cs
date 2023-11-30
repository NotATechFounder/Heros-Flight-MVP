using HeroesFlight.System.Combat.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public abstract class Effect : ScriptableObject
    {
        public abstract T GetData<T>() where T : class;
    }
}