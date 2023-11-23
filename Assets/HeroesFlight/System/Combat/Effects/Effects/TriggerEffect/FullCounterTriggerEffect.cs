using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "FullCounterEffect", menuName = "Combat/Effects/Trigger/FullCounter", order = 100)]
    public class FullCounterTriggerEffect : TriggerEffect
    {
        [SerializeField] private FullCounterData Data;
        public override T GetData<T>()
        {
            return Data as T;
        }
        
        private void OnValidate()
        {
            effectType = EffectType.FullCounter;
        }
    }
}