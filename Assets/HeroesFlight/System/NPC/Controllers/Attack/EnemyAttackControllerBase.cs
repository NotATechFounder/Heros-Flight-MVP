using System;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyAttackControllerBase : MonoBehaviour, AiSubControllerInterface, IAttackControllerInterface
    {
        protected AiControllerInterface aiController;
        protected IHealthController target;
        protected float attackRange;
        protected float timeBetweenAttacks;
        protected float timeSinceLastAttack;
        protected AiAnimatorInterface animator;
        protected IHealthController health;
        protected float currentDamage;
        protected OverlapChecker damageZone;

        public event Action OnHitTarget;
        public event Action<AttackControllerState> OnStateChange;

        public float Damage => currentDamage;
        public float TimeSinceLastAttack => timeSinceLastAttack;

        protected virtual void Start()
        {
            aiController = GetComponent<AiControllerBase>();
            animator = GetComponent<AiAnimatorInterface>();
            damageZone = GetComponentInChildren<OverlapChecker>();
            if (damageZone != null)
                damageZone.OnDetect += DealDamage;
            
            animator.OnAnimationEvent += HandleAnimationEvents;
            health = GetComponent<IHealthController>();
            health.OnDeath += HandleDeath;
            target = aiController.CurrentTarget.GetComponent<IHealthController>();
            timeSinceLastAttack = 0;
            timeBetweenAttacks = aiController.GetMonsterStatModifier()
                .CalculateAttackSpeed(aiController.AgentModel.AiData.AttackSpeed);
            attackRange = aiController.AgentModel.AiData.AttackRange;
            currentDamage = aiController.GetDamage;
        }

        void HandleDeath(IHealthController obj)
        {
            animator.StopAttackAnimation();
        }

        protected virtual void Update()
        {
            if (health.IsDead())
                return;

            if (target.IsDead())
            {
               return;
            }

            timeSinceLastAttack += Time.deltaTime;
        }

        protected virtual void InitAttack(Action onComplete)
        {
            timeSinceLastAttack = 0;
            OnStateChange?.Invoke(AttackControllerState.Attacking);
            animator.StartAttackAnimation(onComplete);
        }

        protected virtual void DealDamage(int i, Collider2D[] collider2Ds)
        {
            var baseDamage = Damage;
            
            float criticalChance = aiController.AgentModel.AiData.CriticalHitChance;
            bool isCritical = Random.Range(0, 100) <= criticalChance;

            float damageToDeal = isCritical
                ? baseDamage * aiController.AgentModel.AiData.CriticalHitChance
                : baseDamage;

            var type = isCritical ? DamageType.Critical : DamageType.NoneCritical;
            var damageModel = new HealthModificationIntentModel(damageToDeal, type, 
                AttackType.Regular,DamageCalculationType.Flat);
            target.TryDealDamage(damageModel);
            OnHitTarget?.Invoke();
        }

        public virtual void AttackTargets(Action OnComplete)
        {
            if (CanAttack())
            {
                InitAttack(OnComplete);
            }
        }

        public void Init()
        {
        }

        public virtual void ToggleControllerState(bool isEnabled)
        {
        }

        public bool CanAttack()
        {
            return timeSinceLastAttack >= timeBetweenAttacks;
        }

        protected virtual void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            OnStateChange?.Invoke(AttackControllerState.Cooldown);
            damageZone.Detect();
        }
    }
}