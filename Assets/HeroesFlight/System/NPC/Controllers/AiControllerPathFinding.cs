using Pathfinding;
using StansAssets.Foundation.Async;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiControllerPathFinding : AiControllerBase
    {
       
        [SerializeField] bool useKnockback=true;
        Path currentPath;
        IAstarAI ai;
        AIDestinationSetter setter;
        bool isInknockback;
      


        public override void Init(Transform player)
        {
            setter = GetComponent<AIDestinationSetter>();
            attackCollider = GetComponent<Collider2D>();
            ai = GetComponent<IAstarAI>();
            ai.canMove = false;
            ai.maxSpeed = m_Model.CombatModel.Speed;
            base.Init(player);
        }

        public override void Enable()
        {
            base.Enable();
            ai.canMove = true;
        }

        public override void Disable()
        {
            ai.canMove = false;
            setter.target = null;
            rigidBody.velocity = Vector2.zero;
            base.Disable();
        }

        public override void ProcessFollowingState()
        {
            if (isInknockback)
                return;

            ai.canMove = !InAttackRange();
            if (setter.target == null)
                setter.target = CurrentTarget;
        }

        public override void ProcessWanderingState()
        {
            if(isDisabled)
                return;
            setter.target = null;
            ai.canMove = !InAttackRange();
            if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            {
                ai.destination = GetRandomPosition2D();
                ai.SearchPath();
            }
        }

        public override void ProcessKnockBack()
        {
            if (!useKnockback)
                return;
            
            if (isInknockback)
                return;
            isInknockback = true;
            ai.canMove = false;
            var forceVector = currentTarget.position.x >= transform.position.x ? Vector2.left : Vector2.right;
            CoroutineUtility.WaitForSeconds(.1f, () =>
            {
                rigidBody.AddForce(forceVector * knockbackForce);
                CoroutineUtility.WaitForSeconds(.5f, () =>
                {
                    if (rigidBody == null)
                        return;

                    ai.canMove = true;
                    isInknockback = false;
                    rigidBody.velocity = Vector2.zero;
                });
            });
        }

        public override Vector2 GetVelocity()
        {
            if (OutOfAgroRange())
            {
                return ai.velocity.normalized;
            }

            var velocity = CurrentTarget.transform.position.x >= transform.position.x
                ? Vector2.right
                : Vector2.left;
            return velocity;
        }

        Vector2 GetRandomPosition2D()
        {
            return Random.insideUnitCircle * wanderDistance;
        }
    }
}