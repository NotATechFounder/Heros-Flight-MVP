using HeroesFlightProject.System.NPC.Data;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public interface AiMoverInterface
    {
        void Init(AiAgentModel model);
        void MoveToTargetPosition(Vector2 target);
        void MoveToTarget(Transform target);
        void MoveToRandomPosition();
        void SetMovementState(bool canMove);
        void ProcessKnockBack(Vector2 direction,float force,float duration);
        void SetMovementSpeed(float newSpeed);
        Vector2 GetVelocity();
        void Disable();
    }
}