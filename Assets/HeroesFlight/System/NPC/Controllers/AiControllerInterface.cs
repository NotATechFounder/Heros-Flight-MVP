using System;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiControllerInterface
    {
        event Action OnInitialized;
        AiAgentModel AgentMode { get; }
        Transform CurrentTarget { get; }
        void Init(Transform player);
        void Enable();
        void Disable();

    }
}