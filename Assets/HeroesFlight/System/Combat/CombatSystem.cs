using System;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Handlers;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.UI;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
using StansAssets.Foundation.Async;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Combat
{
    public class CombatSystem : CombatSystemInterface
    {
        public CombatSystem(EnvironmentSystemInterface environmentSystemInterface,IUISystem uiSystem)
        {
            environmentSystem = environmentSystemInterface;
            this.uiSystem=uiSystem;
            comboHandler = new CharacterComboHandler();
            comboHandler.OnComboUpdated += UpdateCharacterComboUi;
            tick = new WaitForSeconds(1f);
        }

        public event Action<EntityDeathModel> OnEntityDied;

        public event Action<EntityDamageReceivedModel> OnEntityReceivedDamage;


        EnvironmentSystemInterface environmentSystem;

        IUISystem uiSystem;

        CharacterSkillHandler characterSkillHandler;
        CharacterComboHandler comboHandler;
        WorldBarUI specialBar;

        private Coroutine tickRoutine;
        private WaitForSeconds tick;
        Dictionary<IHealthController,CombatEntityModel>  combatEntities= new ();
        private Action OnTick;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            tickRoutine = CoroutineUtility.Start(TickRoutine());
        }

        public void Reset()
        {
            CoroutineUtility.Stop(tickRoutine);
        }


        public void RegisterEntity(CombatEntityModel model)
        {
            if (combatEntities.TryGetValue(model.HealthController, out var combatModel)) return;
                
            combatEntities.Add(model.HealthController,model);
            model.HealthController.Init();
            model.AttackController?.Init();
            model.HealthController.OnDamageReceiveRequest += HandleEntityDamaged;
            model.HealthController.OnDeath += HandleEntityDied;
            OnTick += model.EffectHandler.ExecuteTick;
            if (model.EntityType == CombatEntityType.Player)
            {
                characterSkillHandler =
                    new CharacterSkillHandler(model.HealthController.HealthTransform.GetComponent<CharacterAbilityInterface>());
                specialBar = model.AttackController.specialBar;
                specialBar.ChangeValue(characterSkillHandler.CharacterUltimate.CurrentCharge);
            }
        }

        private void HandleEntityDamaged(HealthModificationRequestModel requestModel)
        {
            if (combatEntities.TryGetValue(requestModel.RequestOwner, out var data))
            {
                if (requestModel.IntentModel.CalculationType == CalculationType.Percentage)
                {
                    var modificationValue = requestModel.RequestOwner.MaxHealth / 100 * requestModel.IntentModel.Amount;
                    requestModel.IntentModel.ModifyAmount(modificationValue); 
                }

                if (requestModel.IntentModel.Attacker != null)
                {
                    if (combatEntities.TryGetValue(requestModel.IntentModel.Attacker, out var model))
                    {
                        if (model.EffectHandler != null)
                        {
                            model.EffectHandler.TriggerCombatEffect(CombatEffectApplicationType.OnDealDamage,
                                requestModel);
                        }
                    }
                }
                
                data.EffectHandler?.TriggerCombatEffect(CombatEffectApplicationType.OnTakeDamage,
                    requestModel);

                requestModel.RequestOwner.ModifyHealth(requestModel.IntentModel);
                switch (data.EntityType)
                {
                    case CombatEntityType.Player:
                        HandlePlayerHealthModified(requestModel);
                        break;
                    case CombatEntityType.Mob:
                        HandleAiDamaged(requestModel);
                        break;
                    case CombatEntityType.MiniBoss:
                        HandleAiDamaged(requestModel);
                        uiSystem.UpdateSpecialEnemyHealthBar(requestModel.RequestOwner.CurrentHealthProportion);
                        break;
                    case CombatEntityType.Boss:
                        HandleAiDamaged(requestModel);
                        break;
                    case CombatEntityType.TempMob:
                        HandleAiDamaged(requestModel);
                        break;
                }


                if (requestModel.IntentModel.AttackType == AttackType.DoT)
                    return;
                
                
                OnEntityReceivedDamage?.Invoke(new EntityDamageReceivedModel(requestModel.RequestOwner.HealthTransform.position,
                    requestModel.IntentModel,data.EntityType,  requestModel.RequestOwner.CurrentHealthProportion ));
            }
        }

        private void HandlePlayerHealthModified(HealthModificationRequestModel obj)
        {
            if(obj.IntentModel.Amount==0)
                return;
            
            uiSystem.ShowDamageText(obj.IntentModel.Amount, obj.RequestOwner.HealthTransform,
                obj.IntentModel.DamageCritType == DamageCritType.Critical, true,
                obj.IntentModel.AttackType == AttackType.Healing);
        }

        private void HandleAiDamaged(HealthModificationRequestModel obj)
        {
            uiSystem.ShowDamageText(obj.IntentModel.Amount, obj.RequestOwner.HealthTransform,
                obj.IntentModel.DamageCritType == DamageCritType.Critical, false);
            comboHandler.RegisterCharacterHit();
            switch (obj.IntentModel.AttackType)
            {
                case AttackType.Regular:

                    characterSkillHandler.CharacterUltimate.UpdateAbilityCharges(5);
                    specialBar.ChangeValue(characterSkillHandler.CharacterUltimate.CurrentCharge);
                    uiSystem.UpdateUltimateButton(characterSkillHandler.CharacterUltimate.CurrentCharge);
                    break;
            }
        }

        private void HandleEntityDied(IHealthController obj)
        {
            if (combatEntities.TryGetValue(obj, out var data))
            {
                OnEntityDied?.Invoke(new EntityDeathModel(obj.HealthTransform.position,data.EntityType));
                if (data.EntityType != CombatEntityType.Player)
                {
                    obj.OnDamageReceiveRequest -= HandleEntityDamaged;
                    obj.OnDeath -= HandleEntityDied;
                    OnTick -= data.EffectHandler.ExecuteTick;
                    combatEntities.Remove(obj);
                }
              
            }
        }

        public void RevivePlayer(float healthPercentage)
        {
            foreach (var data in combatEntities)
            {
                if (data.Value.EntityType == CombatEntityType.Player)
                {
                    data.Key.Revive(healthPercentage);
                }
            }
        }

        public void InitCharacterUltimate(List<AnimationData> animations, int charges)
        {
            characterSkillHandler.CharacterUltimate.Init(animations,charges);
        }

        public void UseCharacterUltimate(Action onBeforeUse=null,Action onComplete = null)
        {
            onBeforeUse?.Invoke();
            characterSkillHandler.CharacterUltimate.UseAbility(onComplete);
            specialBar.ChangeValue(0);
        }

        public void SetSpecialBarValue(float value)
        {
            specialBar.ChangeValue(value);
            uiSystem.UpdateUltimateButton(value);
        }

        public void StartCharacterComboCheck()
        {
           comboHandler.StartComboCheck();
        }

        public void RestartCharacterComboCheck()
        {
            comboHandler.ResetComboCheck();
            comboHandler.StartComboCheck();
        }

        public void ResetCharacterComboCheck()
        {
            comboHandler.ResetComboCheck();
        }

        void UpdateCharacterComboUi(int value)
        {
            uiSystem.UpdateComboUI(value);
        }

        IEnumerator TickRoutine()
        {
            while (true)
            {
               OnTick?.Invoke();
            
                yield return  tick;    
            }
            
        }
    }
}