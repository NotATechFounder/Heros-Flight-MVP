using Pathfinding;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiControllerPathFinding : AiControllerBase
    {
        Path currentPath;
        IAstarAI ai;
        AIDestinationSetter setter;


        public override void Init(Transform player)
        {
            setter = GetComponent<AIDestinationSetter>();
            ai = GetComponent<IAstarAI>();
            ai.maxSpeed = m_Model.CombatModel.Speed;
            base.Init(player);
        }

        public override void Enable()
        {
            ai.canMove = true;
            base.Enable();
        }

        public override void Disable()
        {
            ai.canMove = false;
            base.Disable();
        }

        public override void ProcessFollowingState()
        {
            ai.canMove = !InAttackRange();
            if( setter.target==null)
                setter.target = CurrentTarget;
        }

        public override void ProcessWanderingState()
        {
            setter.target = null;
            if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            {
                ai.destination = GetRandomPosition2D();
                ai.SearchPath();
            }
        }
        
        Vector2 GetRandomPosition2D()
        {
            return Random.insideUnitCircle * wanderDistance;
        }
    }
}