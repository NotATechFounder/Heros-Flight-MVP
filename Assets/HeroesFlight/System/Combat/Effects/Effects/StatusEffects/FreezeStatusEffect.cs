using HeroesFlight.System.Combat.Effects.Effects.Data;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "FreezeEffect", menuName = "Combat/Effects/StatusEffects/Freeze", order = 100)]
    public class FreezeStatusEffect : StatusEffect
    {
        [SerializeField] private FreezeEffectData Data= new FreezeEffectData();

        public override T GetData<T>()
        {
            return Data as T;
        }

     
    }
}