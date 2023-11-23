using HeroesFlight.System.Combat.Effects.Effects.Data;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "ShockEffect", menuName = "Combat/Effects/StatusEffects/Shock", order = 100)]
    public class ShockStatusEffect : StatusEffect
    {
        [SerializeField] private ShockEffectData Data;
        public override T GetData<T>()
        {
            return Data as T;
        }
    }
}