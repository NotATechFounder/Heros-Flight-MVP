using System;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EnemyAttackControllerBase : MonoBehaviour,IAttackControllerInterface
    {
        AiControllerInterface aiController;
        IHealthController target;
        float timeBetweenAttacks;
        float timeSinceLastAttack;
        
      

        public int Damage => aiController.AgentModel.CombatModel.Damage;
        public float TimeSinceLastAttack => timeSinceLastAttack;

        void Start()
        {
            aiController = GetComponent<AiController>();
            target = aiController.CurrentTarget.GetComponent<IHealthController>();
            timeSinceLastAttack = 0;
            timeBetweenAttacks = aiController.AgentModel.CombatModel.TimeBetweenAttacks;

        }

        protected virtual void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
        }
        
        protected  void InitAttack()
        {
           target.DealDamage(Damage);
        }
        public virtual void AttackTarget()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                timeSinceLastAttack = 0;
                InitAttack();
            }
          

        }
    
    }
}