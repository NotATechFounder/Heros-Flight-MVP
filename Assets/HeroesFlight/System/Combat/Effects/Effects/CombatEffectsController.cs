using System;
using System.Collections.Generic;
using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Combat.StatusEffects.Enum;
using HeroesFlightProject.System.Gameplay.Controllers;
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

        protected Dictionary<StatusEffectType, StatusEffectRuntimeModel> statusEffectsMap = new();

        protected IHealthController healthController;
        protected IAttackControllerInterface attackController;
        protected List<StatusEffectType> effectsEndedLastFrame = new();

        protected virtual void Awake()
        {
            healthController = GetComponent<HealthController>();
            attackController = GetComponent<IAttackControllerInterface>();
        }

        public virtual void ApplyStatusEffect(StatusEffect effect)
        {
            if (statusEffectsMap.TryGetValue(effect.EffectType, out var targetModel))
            {
                if (effect.DurationType == EffectDurationType.Instant)
                    return;

                if (targetModel.Effect.IsStackable)
                {
                    targetModel.AddStack();
                }
                else
                {
                    targetModel.RefreshDuration();
                }
            }
            else
            {
                var visual = effect.Visual == null ? null : Instantiate(effect.Visual, visualsParent);
                var newModel = new StatusEffectRuntimeModel(effect, visual);
                newModel.OnEnd += HandleStatusEffectEnd;
                newModel.OnTick += HandleStatusEffectTick;
                statusEffectsMap.Add(effect.EffectType, newModel);

                if (effect.DurationType == EffectDurationType.Instant)
                {
                    HandleStatusEffectTick(new StatusEffectRuntimeModel(effect, visual));
                    effectsEndedLastFrame.Add(effect.EffectType);
                }
            }
        }

        public void ExecuteTick()
        {
            foreach (var effects in effectsEndedLastFrame)
            {
                Destroy(statusEffectsMap[effects].Visual);
                if (statusEffectsMap.ContainsKey(effects))
                    statusEffectsMap.Remove(effects);
            }

            effectsEndedLastFrame.Clear();

            foreach (var data in statusEffectsMap)
            {
                data.Value.ExecuteTick();
            }
        }

        public void AddCombatEffect(CombatEffect effect)
        {
            var visual = effect.Visual == null ? null : Instantiate(effect.Visual, visualsParent);
            if (effectsMap.TryGetValue(effect.ApplyType, out var effects))
            {
                if (!effects.ContainsKey(effect.ID))
                {
                    effects.Add(effect.ID, new CombatEffectRuntimeModel(effect, visual));
                }
            }
            else
            {
                effectsMap.Add(effect.ApplyType, new Dictionary<string, CombatEffectRuntimeModel>());
                effectsMap[effect.ApplyType].Add(effect.ID, new CombatEffectRuntimeModel(effect, visual));
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
                    Debug.Log(model.Effect.ID);
                    foreach (var status in model.Effect.StatusEffects)
                    {
                        var rng = Random.Range(0, 100);
                        if (rng <= status.TriggerChance)
                        {
                            effectsHandler.ApplyStatusEffect(status);
                        }
                        
                    }
                }
            }
        }

        protected virtual void HandleStatusEffectTick(StatusEffectRuntimeModel effectModel)
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