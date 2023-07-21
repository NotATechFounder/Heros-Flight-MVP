using System;
using HeroesFlight.Common.Enum;
using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class HealthController : MonoBehaviour,IHealthController
    {
        [SerializeField] CombatTargetType targetType;
        AiControllerInterface aiController;
        public CombatTargetType TargetType => targetType;
        public int CurrentHealth => currentHealh;
        int maxHealth;
        int currentHealh;
        public void Init()
        {
            aiController = GetComponent<AiControllerInterface>();
            maxHealth = aiController.AgentModel.CombatModel.Health;
            currentHealh = maxHealth;
        }

        public event Action OnBeingDamaged;
        public event Action OnDeath;

        public void DealDamage(int damage)
        {
            Debug.Log($"received {damage}");
        }

        public bool IsDead()
        {
            throw new NotImplementedException();
        }
    }

}
