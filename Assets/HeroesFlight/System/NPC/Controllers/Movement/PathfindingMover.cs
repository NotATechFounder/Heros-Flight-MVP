using HeroesFlight.System.NPC.Controllers.Movement;
using HeroesFlightProject.System.NPC.Enum;
using Pathfinding;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.NPC.Controllers.Movement
{
    public class PathfindingMover : AiBaseMovementController
    {
        IAstarAI ai;
        AIDestinationSetter setter;
        Coroutine knockBackRoutine;

        private void Awake()
        {
            setter = GetComponent<AIDestinationSetter>();
            ai = GetComponent<IAstarAI>();
            ai.canMove = false;
          //  ai.maxSpeed =m_Model.AiData.MoveSpeed;
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

        public override void SetMovementState(bool canMove)
        {
            ai.canMove = canMove;
            base.SetMovementState(base.canMove);
        }
        
        
        Vector2 GetRandomPosition2D()
        {
            var point = Random.insideUnitCircle * model.WanderingDistance;
            if (model.EnemySpawmType == SpawnType.GroundMob)
                point.y = 0;
            point += (Vector2)ai.position;
            return point;
        }
    }
}