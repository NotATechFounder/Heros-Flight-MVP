using System;
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


        public int Damage => aiController.AgentModel.CombatModel.Damage;
        public float TimeSinceLastAttack => timeSinceLastAttack;

        void Start()
        {
            aiController = GetComponent<AiControllerBase>();
            target = aiController.CurrentTarget.GetComponent<IHealthController>();
            timeSinceLastAttack = 0;
            timeBetweenAttacks = aiController.AgentModel.CombatModel.TimeBetweenAttacks;
            attackRange = aiController.AgentModel.CombatModel.AttackRange;
            aiController.SetAttackState(true);
        }

        protected virtual void Update()
        {
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
            target.DealDamage(Damage);
        }

        public virtual void AttackTarget()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                InitAttack();
            }
        }
    }
}