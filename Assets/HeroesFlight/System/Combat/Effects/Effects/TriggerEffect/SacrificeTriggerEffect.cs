using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using UnityEngine;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    [CreateAssetMenu(fileName = "SacrificeEffect", menuName = "Combat/Effects/Trigger/Sacrifice", order = 100)]
    public class SacrificeTriggerEffect : TriggerEffect
    {
        [SerializeField] private SacrificeEffectData Data;
        public override T GetData<T>()
        {
            return Data as T;
        }

        private void OnValidate()
        {
            effectType = EffectType.Sacrifice;
        }
    }
}