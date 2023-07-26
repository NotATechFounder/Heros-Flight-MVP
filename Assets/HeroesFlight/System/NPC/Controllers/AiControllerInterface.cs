using System;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiControllerInterface
    {
        EnemyType EnemyType { get; }
        event Action OnInitialized;
        event Action OnDisabled;
        AiAgentModel AgentModel { get; }
        Transform CurrentTarget { get; }
        
        Vector2 GetVelocity();

        void SetAttackState(bool canAttack);
        void ProcessKnockBack();
        void Init(Transform player);
        void Enable();
        void Disable();

    }
}