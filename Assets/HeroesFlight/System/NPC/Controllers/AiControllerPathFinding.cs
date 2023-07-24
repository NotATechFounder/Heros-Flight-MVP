using Pathfinding;
using StansAssets.Foundation.Async;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiControllerPathFinding : AiControllerBase
    {
        [SerializeField] bool isFLying;
        Path currentPath;
        IAstarAI ai;
        AIDestinationSetter setter;
        bool isInknockback;


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
            if (isInknockback)
                return;

            ai.canMove = !InAttackRange();
            if (setter.target == null)
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

        public override void ProcessKnockBack()
        {
            if (isInknockback)
                return;

            ai.canMove = false;
            var forceVector = currentTarget.position.x >= transform.position.x ? Vector2.left : Vector2.right;
            CoroutineUtility.WaitForEndOfFrame(() =>
            {
                isInknockback = true;
                rigidBody.AddForce(forceVector * knockbackForce);
                CoroutineUtility.WaitForSeconds(.5f, () =>
                {
                    if (isFLying)
                        rigidBody.velocity = Vector2.zero;
                    ai.canMove = true;
                    isInknockback = false;
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

        void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log(col.gameObject.name);
        }
    }
}