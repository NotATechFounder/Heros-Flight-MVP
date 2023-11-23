using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterHealthController : HealthController
    {
        CharacterControllerInterface controller;
        BaseCharacterAttackController attackController;
        CharacterAnimationControllerInterface animator;
        CharacterStatController characterStatController;
        
        private void Awake()
        {
            characterStatController = GetComponent<CharacterStatController>();
        }

        public override void Init()
        {
            controller = GetComponent<CharacterControllerInterface>();
            attackController = GetComponent<BaseCharacterAttackController>();
            animator = GetComponent<CharacterAnimationControllerInterface>();
            animator.PlayIdleAnimation();
            animator = GetComponent<CharacterAnimationController>();
            maxHealth = characterStatController.CurrentMaxHealth;
            characterStatController.OnHealthModified += Heal;
            characterStatController.GetCurrentHealth = () => currentHealth;
            characterStatController.OnMaxHealthChanged = SetMaxHealth;
            base.Init();
        }

      
        protected override void ProcessDeath()
        {
            controller.SetActionState(false);
            attackController.ToggleControllerState(false);
            animator.PlayDeathAnimation();
            base.ProcessDeath();
        }

        public override void Reset()
        {
            controller.SetActionState(true);
            attackController.ToggleControllerState(true);
            animator.PlayDeathAnimation();
            base.Reset();
        }

        public override void Revive(float healthPercentage)
        {
            controller.SetActionState(true);
            attackController.ToggleControllerState(true);
            animator.PlayIdleAnimation();
            base.Revive(healthPercentage);
        }

        public override void TryDealDamage(HealthModificationIntentModel healthModificationIntent)
        {
            dodgeChance = characterStatController.CurrentDodgeChance;
            defence = characterStatController.CurrentDefense;
            base.TryDealDamage(healthModificationIntent);
        }
    }
}