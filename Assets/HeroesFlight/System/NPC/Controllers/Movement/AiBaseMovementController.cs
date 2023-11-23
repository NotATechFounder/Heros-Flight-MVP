using HeroesFlightProject.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Data;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;


namespace HeroesFlight.System.NPC.Controllers.Movement
{
    public abstract class AiBaseMovementController : MonoBehaviour, AiMoverInterface,AiSubControllerInterface
    {
        protected bool isDisabled;
        protected bool canMove;
        protected AiAgentModel model;
        
        public virtual void Init(AiAgentModel model)
        {
            this.model = model;
        }

        public virtual void MoveToTargetPosition(Vector2 target) { }
        public virtual void MoveToTarget(Transform target) { }

        public virtual void MoveToRandomPosition() { }

        public virtual void ProcessKnockBack(Vector2 direction,float force,float duration) { }
        public virtual void SetMovementSpeed(float newSpeed) { }

        public virtual Vector2 GetVelocity()
        {
            return Vector2.zero;
        }

        public void Disable()
        {
            isDisabled = true;
        }

        public virtual void SetMovementState(bool canMove)
        {
            this.canMove = canMove;
        }
    }
}