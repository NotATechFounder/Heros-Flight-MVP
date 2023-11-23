using System.Collections;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using Pathfinding;
using UnityEngine;


namespace HeroesFlight.System.NPC.Controllers.Movement
{
    public class PathfindingMover : AiBaseMovementController
    {
        IAstarAI ai;
        AIDestinationSetter setter;
        Coroutine knockBackRoutine;
        private Rigidbody2D rigiBody;
        private bool isInknockback;

        public override void Init(AiAgentModel model)
        {
            base.Init(model);
            setter = GetComponent<AIDestinationSetter>();
            ai = GetComponent<IAstarAI>();
            ai.canMove = false;
            ai.maxSpeed =model.AiData.MoveSpeed;
            rigiBody = GetComponent<Rigidbody2D>();
        }

      

        public override void MoveToTargetPosition(Vector2 target)
        {
            
            if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            {
                ai.destination = target;
                ai.SearchPath();
            }
        }

        public override void MoveToRandomPosition()
        {
            if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            {
                ai.destination = GetRandomPosition2D();
                ai.SearchPath();
            }
        }

        public override void MoveToTarget(Transform target)
        {
            if (setter.target == target)
                return;
            
            setter.target = target;
        }

        public override void SetMovementState(bool canMove)
        {
            base.SetMovementState(canMove);
            if (isDisabled)
            {
                ai.isStopped = true;
                ai.canMove = false;
            }
            else
            {
                ai.isStopped = !canMove;
                ai.canMove = canMove;    
            }
            
        }
        
        public override Vector2 GetVelocity()
        {
            return ai.velocity.normalized;
        }
        
      
        Vector2 GetRandomPosition2D()
        {
            var point = Random.insideUnitCircle * model.WanderingDistance;
            if (model.EnemySpawmType == SpawnType.GroundMob)
                point.y = 0;
            point += (Vector2)ai.position;
            return point;
        }

        public override void SetMovementSpeed(float newSpeed)
        {
            ai.maxSpeed = newSpeed;
        }

        public override void ProcessKnockBack(Vector2 direction,float force,float duration)
        {
            if (isInknockback || !canMove)
                return;
            isInknockback = true;
            SetMovementState(false);
            knockBackRoutine = StartCoroutine(KnockBackRoutine(direction,force,duration));
        }
        
        IEnumerator KnockBackRoutine(Vector2 forceVector,float force,float duration)
        {
            yield return new WaitForEndOfFrame();
            rigiBody.AddForce(forceVector * force, ForceMode2D.Impulse);
            yield return new WaitForSeconds(duration);
            
            isInknockback = false;
            rigiBody.velocity = Vector2.zero;
            SetMovementState(true);
        }
    }
}