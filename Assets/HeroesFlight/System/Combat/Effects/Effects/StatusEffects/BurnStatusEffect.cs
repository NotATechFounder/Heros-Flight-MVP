using HeroesFlight.System.Combat.Effects.Effects.Data;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "BurnEffect", menuName = "Combat/Effects/StatusEffects/Burn", order = 100)]
    public class BurnStatusEffect : StatusEffect
    {
        [SerializeField] private BurnEffectData data;
        public override T GetData<T>()
        {
            return data as T;
        }
    }
}