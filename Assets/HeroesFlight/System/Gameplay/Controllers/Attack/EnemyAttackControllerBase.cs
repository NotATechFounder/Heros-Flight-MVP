using System;
using HeroesFlight.Common.Animation;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Gameplay.Enum;
using HeroesFlight.System.Gameplay.Model;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyAttackControllerBase : MonoBehaviour, IAttackControllerInterface
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

        public float Damage => currentDamage;
        public float TimeSinceLastAttack => timeSinceLastAttack;

        void Start()
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
                .CalculateAttackSpeed(aiController.AgentModel.CombatModel.GetMonsterStatData.AttackSpeed);
            attackRange = aiController.AgentModel.CombatModel.GetMonsterStatData.AttackRange;
            aiController.SetAttackState(true);
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
                aiController.SetAttackState(false);
                return;
            }

            timeSinceLastAttack += Time.deltaTime;
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                aiController.SetAttackState(true);
            }
        }

        protected virtual void InitAttack()
        {
            Debug.Log("STARTING ATTACK");
            timeSinceLastAttack = 0;
            animator.StartAttackAnimation(null);
        }

        protected virtual void DealDamage(int i, Collider2D[] collider2Ds)
        {
            Debug.Log("detected player");
            target.DealDamage(new DamageModel(Damage, DamageType.NoneCritical, AttackType.Regular));
            aiController.SetAttackState(false);
            OnHitTarget?.Invoke();
        }

        public virtual void AttackTargets()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                InitAttack();
            }
        }

        public void Init()
        {
        }

        public virtual void ToggleControllerState(bool isEnabled)
        {
        }

        protected virtual void HandleAnimationEvents(AttackAnimationEvent obj)
        {
            damageZone.Detect();
        }
    }
}