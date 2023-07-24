using System;
using HeroesFlightProject.System.NPC.Data;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiControllerInterface
    {
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