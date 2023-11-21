using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "ReflectEffect", menuName = "Combat/Effects/Trigger/Reflect", order = 100)]
    public class ReflectTriggerEffect : TriggerEffect
    {
        [SerializeField] private ReflectEffectData Data;

        public override T GetData<T>()
        {
            return Data as T;
        }
        
        private void OnValidate()
        {
            effectType =  EffectType.Reflect;
        }
    }
}