using System;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Combat.Handlers;
using HeroesFlight.System.Combat.Model;
using HeroesFlight.System.Environment;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.UI;
using HeroesFlightProject.System.Combat.Controllers;
using HeroesFlightProject.System.Gameplay.Controllers;
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
        }

        public event Action<EntityDeathModel> OnEntityDied;

        public event Action<EntityDamageReceivedModel> OnEntityReceivedDamage;


        EnvironmentSystemInterface environmentSystem;

        IUISystem uiSystem;

        CharacterSkillHandler characterSkillHandler;
        CharacterComboHandler comboHandler;

        Dictionary<IHealthController,CombatEntityModel>  combatEntities= new ();

        public void Init(Scene scene = default, Action onComplete = null) {}

        public void Reset() { }


        public void RegisterEntity(CombatEntityModel model)
        {
            if (combatEntities.TryGetValue(model.HealthController, out var combatModel)) return;
                
            combatEntities.Add(model.HealthController,model);
            model.HealthController.Init();
            model.AttackController?.Init();
            model.HealthController.OnDamageReceiveRequest += HandleEntityDamaged;
            model.HealthController.OnDeath += HandleEntityDied;
            if (model.EntityType == CombatEntityType.Player)
            {
                characterSkillHandler =
                    new CharacterSkillHandler(model.HealthController.HealthTransform.GetComponent<CharacterAbilityInterface>());    
            }
        }

        private void HandleEntityDamaged(HealthModificationRequestModel obj)
        {
            if (combatEntities.TryGetValue(obj.RequestOwner, out var data))
            {
                obj.RequestOwner.ModifyHealth(obj.IntentModel);
                switch (data.EntityType)
                {
                    case CombatEntityType.Player:
                        HandlePlayerHealthModified(obj);
                        break;
                    case CombatEntityType.Mob:
                        HandleAiDamaged(obj);
                        break;
                    case CombatEntityType.MiniBoss:
                        HandleAiDamaged(obj);
                        break;
                    case CombatEntityType.Boss:
                        HandleAiDamaged(obj);
                        break;
                    case CombatEntityType.TempMob:
                        HandleAiDamaged(obj);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                OnEntityReceivedDamage?.Invoke(new EntityDamageReceivedModel(obj.RequestOwner.HealthTransform.position,
                    obj.IntentModel,data.EntityType,  obj.RequestOwner.CurrentHealthProportion ));
            }
        }

        private void HandlePlayerHealthModified(HealthModificationRequestModel obj)
        {
            uiSystem.ShowDamageText(obj.IntentModel.Amount, obj.RequestOwner.HealthTransform,
                obj.IntentModel.DamageType == DamageType.Critical, true,
                obj.IntentModel.AttackType == AttackType.Healing);
        }

        private void HandleAiDamaged(HealthModificationRequestModel obj)
        {
            uiSystem.ShowDamageText(obj.IntentModel.Amount, obj.RequestOwner.HealthTransform,
                obj.IntentModel.DamageType == DamageType.Critical, false);
            comboHandler.RegisterCharacterHit();
            switch (obj.IntentModel.AttackType)
            {
                case AttackType.Regular:

                    characterSkillHandler.CharacterUltimate.UpdateAbilityCharges(5);
                    uiSystem.UpdateUltimateButtonFill(characterSkillHandler.CharacterUltimate.CurrentCharge);
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
                    combatEntities.Remove(obj);
                }
              
            }
        }

        public void RevivePlayer()
        {
            foreach (var data in combatEntities)
            {
                if (data.Value.EntityType == CombatEntityType.Player)
                {
                    data.Key.Revive();
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
    }
}