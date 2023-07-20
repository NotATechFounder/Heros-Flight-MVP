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
        void Start()
        {
            aiController = GetComponent<AiController>();
            target = aiController.CurrentTarget.GetComponent<IHealthController>();
            timeSinceLastAttack = 0;
            timeBetweenAttacks = aiController.AgentMode.TimeBetweenAttacks;

        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        public event Action<IHealthController> OnDealDamageRequest;
       
        public virtual void AttackTarget()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                timeSinceLastAttack = 0;
                Debug.Log("Attacking player");
                OnDealDamageRequest?.Invoke(target);
            }
          

        }
    
    }
}