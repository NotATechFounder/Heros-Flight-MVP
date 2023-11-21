using HeroesFlight.System.Combat.Effects.Effects.Data;
using UnityEngine;



namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "PoisonEffect", menuName = "Combat/Effects/StatusEffects/Poison", order = 100)]
    public class PoisonStatusEffect : StatusEffect
    {
        [SerializeField] private PoisonEffectData Data;
        public override T GetData<T>()
        {
            return Data as T;
        }
    }
}