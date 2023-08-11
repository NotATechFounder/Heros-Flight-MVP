using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay.Model;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CharacterHealthController : HealthController
    {
        CharacterControllerInterface controller;
        CharacterAttackController attackController;
        CharacterAnimationControllerInterface animator;
        CharacterStatController characterStatController;

        private void Awake()
        {
            characterStatController = GetComponent<CharacterStatController>();
        }

        public override void Init()
        {
            controller = GetComponent<CharacterControllerInterface>();
            attackController = GetComponent<CharacterAttackController>();
            animator = GetComponent<CharacterAnimationControllerInterface>();
            animator.PlayIdleAnimation();
            animator = GetComponent<CharacterAnimationController>();
            maxHealth = controller.CharacterSO.GetPlayerStatData.Health;
            characterStatController.OnHealthModified += ModifyHealth;
            characterStatController.GetCurrentHealth = () => currentHealth;
            base.Init();
        }

        private void ModifyHealth(float value, bool increase)
        {
            if (increase)
                Heal(value);
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

        public override void Revive()
        {
            controller.SetActionState(true);
            attackController.ToggleControllerState(true);
            animator.PlayDeathAnimation();
            base.Revive();
        }

        public override void DealDamage(DamageModel damage)
        {
            defence = characterStatController.CurrentDefense;
            base.DealDamage(damage);
        }
    }
}