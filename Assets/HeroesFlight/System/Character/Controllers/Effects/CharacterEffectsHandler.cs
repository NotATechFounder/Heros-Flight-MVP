using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using UnityEngine;

namespace HeroesFlight.System.Character.Controllers.Effects
{
    public class CharacterEffectsHandler :CombatEffectsController
    {
        [SerializeField] private List<CombatEffect> testEffect;

        protected override void Awake()
        {
            base.Awake();
            foreach (var effect in testEffect)
            {
                AddCombatEffect(effect);
            }
            Debug.Log(effectsMap.Count);
            Debug.Log(effectsMap[CombatEffectApplicationType.OnDealDamage].Count);
        }

        protected override void HandleStatusEffectTick(StatusEffectRuntimeModel effectModel)
        {
            base.HandleStatusEffectTick(effectModel);

            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Burn:
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value,
                        DamageCritType.NoneCritical,AttackType.Regular,DamageCalculationType.Percentage,null));
                    break;
                case EffectType.Freeze:
                    break;
                case EffectType.Root:
                    break;
                case EffectType.Poison:
                    break;
                case EffectType.Shock:
                    break;
                case EffectType.Reflect:
                    break;
                case EffectType.Sacrifice:
                    break;
                case EffectType.FullCounter:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}