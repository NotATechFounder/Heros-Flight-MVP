using System;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;

namespace HeroesFlight.System.NPC.Controllers.Effects
{
    public class AiEffectsController : CombatEffectsController
    {
        protected override void HandleStatusEffectTick(StatusEffectRuntimeModel effectModel)
        {
          
            switch (effectModel.Effect.EffectType)
            {
                case EffectType.Burn:
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value *effectModel.CurrentStacks ,
                        DamageCritType.NoneCritical,AttackType.DoT,DamageCalculationType.Percentage,null));
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
                    healthController.TryDealDamage(new HealthModificationIntentModel(effectModel.Effect.Value,
                        DamageCritType.NoneCritical,AttackType.DoT,DamageCalculationType.Flat,null));
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