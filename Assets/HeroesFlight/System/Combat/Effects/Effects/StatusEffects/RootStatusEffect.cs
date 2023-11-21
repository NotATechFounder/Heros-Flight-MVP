using HeroesFlight.System.Combat.Effects.Effects.Data;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "RootEffect", menuName = "Combat/Effects/StatusEffects/Root", order = 100)]
    public class RootStatusEffect : StatusEffect
    {
        [SerializeField] private RootEffectData Data;
        public override T GetData<T>()
        {
            return Data as T;
        }
    }
}