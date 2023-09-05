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
        event Action<AiControllerInterface> OnDisabled;
        AiAgentModel AgentModel { get; }
        Transform CurrentTarget { get; }

        public int GetHealth { get; }

        public float GetDamage { get; }

        Vector2 GetVelocity();

        MonsterStatModifier GetMonsterStatModifier();

        void SetAttackState(bool canAttack);
        void ProcessKnockBack();
        void Aggravate();
        void SetMovementState(bool canMove);
        void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon);
        void Enable();
        void Disable();

    }
}