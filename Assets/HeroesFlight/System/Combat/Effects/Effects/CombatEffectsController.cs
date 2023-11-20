using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Combat.Visuals;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlight.System.Combat.Effects.Effects
{
    public class CombatEffectsController : MonoBehaviour, CombatEntityEffectsHandlerInterface
    {
        [SerializeField] private Transform visualsParent;

        protected Dictionary<CombatEffectApplicationType, Dictionary<string, CombatEffectRuntimeModel>> effectsMap =
            new();

        protected Dictionary<EffectType, StatusEffectRuntimeModel> statusEffectsMap = new();

        protected IHealthController healthController;
        protected IAttackControllerInterface attackController;
        protected List<EffectType> effectsEndedLastFrame = new();

        protected virtual void Awake()
        {
            healthController = GetComponent<HealthController>();
            attackController = GetComponent<IAttackControllerInterface>();
        }

        public virtual void ApplyStatusEffect(StatusEffect effect)
        {
            if (statusEffectsMap.TryGetValue(effect.EffectType, out var targetModel))
            {
                if (targetModel.Effect.IsStackable)
                {
                    targetModel.AddStack();
                }
                else
                {
                    if (effect.CanReApply)
                        targetModel.RefreshDuration();
                }
            }
            else
            {
                GameObject visual = null;
                if (effect.Visual != null)
                {
                    switch (effect.EffectType)
                    {
                        case EffectType.Poison:
                            ApplyPoisonEffect(effect, out visual);
                            break;
                        case EffectType.Shock:
                            ApplyShockEffect(effect, out visual);
                            break;
                            
                        case EffectType.Root:
                            ApplyRootEffect(effect, out visual);
                            break;
                        
                        case EffectType.Freeze:
                            ApplyFreezeEffect(effect, out visual);
                            break;
                        default:
                            ApplyDotEffect(effect, out visual);
                            break;
                    }
                }


                var newModel = new StatusEffectRuntimeModel(ConvertStatusToRealEffect(effect), visual);
                newModel.OnEnd += HandleStatusEffectEnd;
                newModel.OnTick += HandleStatusEffectTick;
                statusEffectsMap.Add(effect.EffectType, newModel);
            }
        }

       

        private StatusEffect ConvertStatusToRealEffect(StatusEffect effect)
        {
            switch (effect.EffectType)
            {
                case EffectType.Burn:
                    return effect as BurnStatusEffect;
                    break;
                case EffectType.Freeze:
                    return effect;
                    break;
                case EffectType.Root:
                    return effect;
                    break;
                case EffectType.Poison:
                    return effect;
                    break;
                case EffectType.Shock:
                    return effect;
                    break;
             default:
                 return null;
            }
        }

        public virtual void ExecuteTick()
        {
            foreach (var effectType in effectsEndedLastFrame)
            {
                if (statusEffectsMap[effectType].Visual != null)
                {
                    if (statusEffectsMap[effectType].Visual.TryGetComponent<Particle>(out var particle))
                    {
                        particle.GetParticleSystem.Stop();
                    }
                    else
                    {
                        Destroy(statusEffectsMap[effectType].Visual);
                    }
                }


                if (statusEffectsMap.ContainsKey(effectType))
                {
                    HandleStatusEffectRemoval(statusEffectsMap[effectType]);
                    statusEffectsMap.Remove(effectType);
                }
            }

            effectsEndedLastFrame.Clear();

            foreach (var data in statusEffectsMap)
            {
                data.Value.ExecuteTick();
            }
        }

        public void AddCombatEffect(CombatEffect effect,int lvl)
        {
            var visual = effect.Visual == null ? null : Instantiate(effect.Visual, visualsParent);
            if (effectsMap.TryGetValue(effect.ApplyType, out var effects))
            {
                if (!effects.ContainsKey(effect.ID))
                {
                    effects.Add(effect.ID, new CombatEffectRuntimeModel(effect, visual,lvl));
                }
                else
                {
                    effects.Remove(effect.ID);
                    effects.Add(effect.ID, new CombatEffectRuntimeModel(effect, visual,lvl));
                }
            }
            else
            {
                effectsMap.Add(effect.ApplyType, new Dictionary<string, CombatEffectRuntimeModel>());
                effectsMap[effect.ApplyType].Add(effect.ID, new CombatEffectRuntimeModel(effect, visual,lvl));
            }
        }

        public void RemoveEffect(CombatEffect effect)
        {
            if (effectsMap.TryGetValue(effect.ApplyType, out var effects))
            {
                if (effects.TryGetValue(effect.ID, out var model))
                {
                    effects.Remove(effect.ID);
                    if (model.Visual != null)
                    {
                        Destroy(model.Visual);
                    }
                }
            }
        }

        public void TriggerCombatEffect(CombatEffectApplicationType useType,
            HealthModificationRequestModel requestModel)
        {
            if (effectsMap.TryGetValue(useType, out var effects))
            {
                CombatEffectsController effectsHandler = null;
                switch (useType)
                {
                    case CombatEffectApplicationType.OnTakeDamage:
                        if (requestModel.IntentModel.Source != null)
                        {
                            effectsHandler = requestModel.IntentModel.Source.HealthTransform
                                .GetComponent<CombatEffectsController>();
                        }

                        break;
                    case CombatEffectApplicationType.OnDealDamage:
                        effectsHandler =
                            requestModel.RequestOwner.HealthTransform.GetComponent<CombatEffectsController>();
                        break;
                    case CombatEffectApplicationType.OnInit:
                        break;
                }

                if (effectsHandler == null)
                    return;

                foreach (var model in effects.Values)
                {
                    foreach (var status in model.Effect.StatusEffects)
                    {
                        TryTriggerEffect(status, effectsHandler, requestModel);
                    }
                }
            }
        }

        protected void TryTriggerEffect(Effect status, CombatEffectsController effectsHandler,
            HealthModificationRequestModel healthModificationRequestModel)
        {
            var data = status.GetData<EffectData>();
            var triggerChance = data.ProcChance.GetCurrentValue(1);
            Debug.Log(triggerChance);
            var rng = Random.Range(0, 101);
            if (rng > triggerChance)
                return;

            var type = status.GetType();
            
            if (type.IsSubclassOf( typeof(StatusEffect)))
            {
                effectsHandler.ApplyStatusEffect(status as StatusEffect);
            }
            else if (status.GetType() == typeof(TriggerEffect))
            {
                ActivateEffectOnTarget(status as TriggerEffect, effectsHandler, healthModificationRequestModel);
            }
        }

        protected virtual void ActivateEffectOnTarget(TriggerEffect effect, CombatEffectsController effectsHandler,
            HealthModificationRequestModel healthModificationRequestModel)
        {
            switch (effect.EffectType)
            {
                case EffectType.Burn:
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
                    HandleReflect(effect, healthModificationRequestModel);
                    break;
                case EffectType.Sacrifice:
                    HandleSacrifice(effect, healthModificationRequestModel);
                    break;
                case EffectType.FullCounter:
                    HandleFullCounter(effect, healthModificationRequestModel);
                    break;
            }
        }

        protected virtual void HandleSacrifice(TriggerEffect effect,
            HealthModificationRequestModel healthModificationRequestModel)
        {
            var additionalDamage = attackController.Damage / 100 * effect.Value;
            var healthPercTreshhold = healthController.MaxHealth / 100 * effect.OptionalValue;
            var healthDiff = healthController.MaxHealth - healthController.CurrentHealth;
            var currentHealthPerc = Mathf.FloorToInt(healthDiff / healthPercTreshhold);
            var additionalFinaleDamage = additionalDamage * currentHealthPerc;
            healthModificationRequestModel.IntentModel.ModifyAmount(
                healthModificationRequestModel.IntentModel.Amount + additionalFinaleDamage
            );
        }

        protected virtual void ApplyDotEffect(StatusEffect effect, out GameObject visual)
        {
            visual = ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                visualsParent.position).gameObject;
            visual.transform.SetParent(visualsParent);
        }
        
        protected virtual void ApplyFreezeEffect(StatusEffect effect, out GameObject visual)
        {
            visual = ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                visualsParent.position).gameObject;
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void ApplyPoisonEffect(StatusEffect effect, out GameObject visual)
        {
            visual = ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                visualsParent.position).gameObject;
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void ApplyRootEffect(StatusEffect effect, out GameObject visual)
        {
            visual = Instantiate(effect.Visual, visualsParent);
            visual.transform.SetParent(visualsParent);
            visual.GetComponent<EntanglingRootsVisual>().ShowRootsVisual();
        }

        protected virtual void ApplyShockEffect(StatusEffect effect, out GameObject visual)
        {
            visual = Instantiate(effect.Visual, visualsParent);
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void HandleFullCounter(TriggerEffect effect,
            HealthModificationRequestModel healthModificationRequestModel)
        {
            healthModificationRequestModel.IntentModel.Source
                .TryDealDamage(new HealthModificationIntentModel(attackController.Damage / 100 * effect.Value,
                    DamageCritType.NoneCritical, AttackType.DoT, CalculationType.Flat, null));
            if (effect.Visual != null)
            {
                ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                    healthModificationRequestModel.IntentModel.Source.HealthTransform.position);
            }

            healthModificationRequestModel.IntentModel.ModifyAmount(0);
            PopUpManager.Instance.PopUpAtTextPosition(
                healthModificationRequestModel.IntentModel.Source.HealthTransform.position, Vector3.zero,
                "COUNTER", Color.yellow, 100);
        }

        protected virtual void HandleReflect(TriggerEffect effect,
            HealthModificationRequestModel healthModificationRequestModel)
        {
            var possibleHealthDamage =
                healthModificationRequestModel.IntentModel.Source.MaxHealth / 100 * effect.OptionalValue;
            var finalDamage = possibleHealthDamage > effect.Value ? possibleHealthDamage : effect.Value;
            healthModificationRequestModel.IntentModel.Source
                .TryDealDamage(new HealthModificationIntentModel(finalDamage,
                    DamageCritType.NoneCritical, AttackType.DoT, effect.CalculationType, null));
            if (effect.Visual != null)
            {
                ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                    healthModificationRequestModel.IntentModel.Source.HealthTransform.position);
            }
        }

        protected virtual void HandleStatusEffectTick(StatusEffectRuntimeModel effectModel)
        {
        }

        protected virtual void HandleStatusEffectRemoval(StatusEffectRuntimeModel effectModel)
        {
        }

        private void HandleStatusEffectEnd(StatusEffectRuntimeModel statusEffectRuntimeModel)
        {
            if (statusEffectsMap.TryGetValue(statusEffectRuntimeModel.Effect.EffectType, out var targetModel))
            {
                effectsEndedLastFrame.Add(targetModel.Effect.EffectType);
            }
        }
    }
}