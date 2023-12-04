using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlight.System.Combat.Visuals;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.Gameplay.Controllers;
using Pelumi.ObjectPool;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
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

        public virtual void ApplyStatusEffect(StatusEffect effect, int modelLvl)
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
                            ApplyPoisonEffect(effect, out visual,modelLvl);
                            break;
                        case EffectType.Shock:
                            ApplyShockEffect(effect, out visual,modelLvl);
                            break;

                        case EffectType.Root:
                            ApplyRootEffect(effect, out visual,modelLvl);
                            break;

                        case EffectType.Freeze:
                            ApplyFreezeEffect(effect, out visual,modelLvl);
                            break;
                        default:
                            ApplyDotEffect(effect, out visual,modelLvl);
                            break;
                    }
                }


                var newModel = new StatusEffectRuntimeModel(ConvertStatusToRealEffect(effect), visual,modelLvl);
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
                  
                case EffectType.Freeze:
                    return effect as FreezeStatusEffect;
                   
                case EffectType.Root:
                    return effect as RootStatusEffect;
                    
                case EffectType.Poison:
                    return effect as PoisonStatusEffect;
                   
                case EffectType.Shock:
                    return effect as ShockStatusEffect;
                    
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

        public void AddCombatEffect(CombatEffect effect, int lvl)
        {
            var visual = effect.Visual == null ? null : Instantiate(effect.Visual, visualsParent);
            if (effectsMap.TryGetValue(effect.ApplyType, out var effects))
            {
                if (!effects.ContainsKey(effect.ID))
                {
                    effects.Add(effect.ID, new CombatEffectRuntimeModel(effect, visual, lvl));
                }
                else
                {
                    if (effect.ApplyType == CombatEffectApplicationType.OnInit)
                    {
                        RemoveCombatEffectsValues(effects[effect.ID]);
                    }

                    effects.Remove(effect.ID);
                    effects.Add(effect.ID, new CombatEffectRuntimeModel(effect, visual, lvl));
                }
            }
            else
            {
                effectsMap.Add(effect.ApplyType, new Dictionary<string, CombatEffectRuntimeModel>());
                effectsMap[effect.ApplyType].Add(effect.ID, new CombatEffectRuntimeModel(effect, visual, lvl));
            }

            if (effect.ApplyType == CombatEffectApplicationType.OnInit)
            {
                ApplyEffectOnInit(effect,lvl);
            }
        }

        public void AddCombatEffect(CombatEffect effect)
        {
            AddCombatEffect(effect,1);
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

                    if (model.Effect.ApplyType == CombatEffectApplicationType.OnInit)
                    {
                        RemoveCombatEffectsValues(model);
                    }
                }
            }
        }

        protected virtual void ApplyEffectOnInit(CombatEffect combatEffect, int lvl) { }

        protected virtual void RemoveCombatEffectsValues(CombatEffectRuntimeModel runtimeEffect) {}

        public void TriggerCombatEffect(CombatEffectApplicationType useType,
            HealthModificationRequestModel requestModel)
        {
            if (effectsMap.TryGetValue(useType, out var effects))
            {
                CombatEffectsController effectsHandler = null;
                switch (useType)
                {
                    case CombatEffectApplicationType.OnTakeDamage:
                        if (requestModel.IntentModel.Attacker != null)
                        {
                            effectsHandler = requestModel.IntentModel.Attacker.HealthTransform
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
                    foreach (var status in model.Effect.EffectToApply)
                    {
                        TryTriggerEffect(status,model.Lvl, effectsHandler, requestModel);
                    }
                }
            }
        }

        protected void TryTriggerEffect(Effect status, int modelLvl, CombatEffectsController effectsHandler,
            HealthModificationRequestModel healthModificationRequestModel)
        {
            var data = status.GetData<EffectData>();
            var triggerChance = data.ProcChance.GetCurrentValue(modelLvl);
            Debug.Log(triggerChance);
            var rng = Random.Range(0, 101);
            if (rng > triggerChance)
                return;

            var type = status.GetType();

            if (type.IsSubclassOf(typeof(StatusEffect)))
            {
                effectsHandler.ApplyStatusEffect(status as StatusEffect,modelLvl);
            }
            else if (type.IsSubclassOf(typeof(TriggerEffect)))
            {
                ActivateEffectOnTarget(status as TriggerEffect, effectsHandler, healthModificationRequestModel,modelLvl);
            }
        }

        protected virtual void ActivateEffectOnTarget(TriggerEffect effect, CombatEffectsController effectsHandler,
            HealthModificationRequestModel healthModificationRequestModel, int modelLvl)
        {
            switch (effect.EffectType)
            {
                case EffectType.Reflect:
                    HandleReflect(effect, healthModificationRequestModel,modelLvl);
                    break;
                case EffectType.Sacrifice:
                    HandleSacrifice(effect, healthModificationRequestModel,modelLvl);
                    break;
                case EffectType.FullCounter:
                    HandleFullCounter(effect, healthModificationRequestModel,modelLvl);
                    break;
            }
        }

        protected virtual void HandleSacrifice(TriggerEffect effect,
            HealthModificationRequestModel healthModificationRequestModel, int modelLvl)
        {
            var data = effect.GetData<SacrificeEffectData>();
            var additionalDamage = attackController.Damage / 100 * data.DamageBoost.GetCurrentValue(modelLvl);
            var healthPercTreshhold = healthController.MaxHealth / 100 * data.HealthThreshhold.GetCurrentValue(modelLvl);
            var healthDiff = healthController.MaxHealth - healthController.CurrentHealth;
            var currentHealthPerc = Mathf.FloorToInt(healthDiff / healthPercTreshhold);
            var additionalFinaleDamage = additionalDamage * currentHealthPerc;
            healthModificationRequestModel.IntentModel.ModifyAmount(
                healthModificationRequestModel.IntentModel.Amount + additionalFinaleDamage
            );
        }

        protected virtual void ApplyDotEffect(StatusEffect effect, out GameObject visual, int modelLvl)
        {
            visual = ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                visualsParent.position).gameObject;
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void ApplyFreezeEffect(StatusEffect effect, out GameObject visual, int modelLvl)
        {
            visual = ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                visualsParent.position).gameObject;
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void ApplyPoisonEffect(StatusEffect effect, out GameObject visual, int modelLvl)
        {
            visual = ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                visualsParent.position).gameObject;
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void ApplyRootEffect(StatusEffect effect, out GameObject visual, int modelLvl)
        {
            visual = Instantiate(effect.Visual, visualsParent);
            visual.transform.SetParent(visualsParent);
            visual.GetComponent<EntanglingRootsVisual>().ShowRootsVisual();
        }

        protected virtual void ApplyShockEffect(StatusEffect effect, out GameObject visual, int modelLvl)
        {
            visual = Instantiate(effect.Visual, visualsParent);
            visual.transform.SetParent(visualsParent);
        }

        protected virtual void HandleFullCounter(TriggerEffect effect,
            HealthModificationRequestModel healthModificationRequestModel, int modelLvl)
        {
            var data = effect.GetData<FullCounterData>();
            healthModificationRequestModel.IntentModel.Attacker
                .TryDealDamage(new HealthModificationIntentModel(
                    attackController.Damage / 100 * data.Damage.GetCurrentValue(modelLvl),
                    DamageCritType.NoneCritical, AttackType.DoT, effect.CalculationType, null));
            if (effect.Visual != null)
            {
                ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                    healthModificationRequestModel.IntentModel.Attacker.HealthTransform.position);
            }

            healthModificationRequestModel.IntentModel.ModifyAmount(0);
            PopUpManager.Instance.PopUpAtTextPosition(
                healthModificationRequestModel.RequestOwner.HealthTransform.position, Vector3.zero,
                "COUNTER", Color.yellow, 100);
        }

        protected virtual void HandleReflect(TriggerEffect effect,
            HealthModificationRequestModel healthModificationRequestModel, int modelLvl)
        {
            var data = effect.GetData<ReflectEffectData>();
            var possibleHealthDamage =
                healthModificationRequestModel.IntentModel.Attacker.MaxHealth / 100 *
                data.PercentageDamage.GetCurrentValue(modelLvl);
            var finalDamage = possibleHealthDamage > data.FlatDamage.GetCurrentValue(modelLvl)
                ? possibleHealthDamage
                :  data.FlatDamage.GetCurrentValue(modelLvl);


            healthModificationRequestModel.IntentModel.Attacker
                .TryDealDamage(new HealthModificationIntentModel(finalDamage,
                    DamageCritType.NoneCritical, AttackType.DoT, effect.CalculationType, null));
            if (effect.Visual != null)
            {
                ParticleManager.instance.Spawn(effect.Visual.GetComponent<Particle>(),
                    healthModificationRequestModel.IntentModel.Attacker.HealthTransform.position);
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