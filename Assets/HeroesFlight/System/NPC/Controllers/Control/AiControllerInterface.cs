using System;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiControllerInterface
    {
        EnemyType EnemyType { get; }
        event Action<AiControllerInterface> OnDisabled;
        AiAgentModel AgentModel { get; }
        Transform CurrentTarget { get; }
        MonsterStatModifier GetMonsterStatModifier();
        void ProcessKnockBack();
        void Aggravate();
        void SetMovementState(bool canMove);
        void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon);
      
        bool TryGetController<T>(out T controller);

    }
}