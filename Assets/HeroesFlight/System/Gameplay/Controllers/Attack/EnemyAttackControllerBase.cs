using System;
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

        public int Damage => aiController.AgentModel.CombatModel.Damage;
        public float TimeSinceLastAttack => timeSinceLastAttack;

        void Start()
        {
            aiController = GetComponent<AiControllerBase>();
            animator = GetComponent<AiAnimatorInterface>();
            health = GetComponent<IHealthController>();
            health.OnDeath += HandleDeath;
            target = aiController.CurrentTarget.GetComponent<IHealthController>();
            timeSinceLastAttack = 0;
            timeBetweenAttacks = aiController.AgentModel.CombatModel.TimeBetweenAttacks;
            attackRange = aiController.AgentModel.CombatModel.AttackRange;
            aiController.SetAttackState(true);
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
            timeSinceLastAttack = 0;
            aiController.SetAttackState(false);
            target.DealDamage(new DamageModel(Damage,DamageType.NoneCritical));
        }

        public virtual void AttackTargets()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                InitAttack();
            }
        }

        public void Init() { }

        public virtual void ToggleControllerState(bool isEnabled) { }
    }
}