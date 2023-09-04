using System.Collections;
using HeroesFlightProject.System.NPC.Enum;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiControllerPathFinding : AiControllerBase
    {
        IAstarAI ai;
        AIDestinationSetter setter;
        Coroutine knockBackRoutine;


        public override void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon)
        {
            setter = GetComponent<AIDestinationSetter>();
            attackCollider = GetComponent<Collider2D>();
            ai = GetComponent<IAstarAI>();
            ai.canMove = false;
            ai.maxSpeed = m_Model.CombatModel.GetMonsterStatData.MoveSpeed;
            base.Init(player, health, damage, monsterStatModifier, currentCardIcon);
        }

        public override void Enable()
        {
            base.Enable();
            ai.canMove = true;
        }

        public override void Disable()
        {
            if (knockBackRoutine != null)
            {
                StopCoroutine(knockBackRoutine);
            }

            ai.canMove = false;
            setter.target = null;
            rigidBody.velocity = Vector2.zero;
            base.Disable();
        }

        void OnDestroy()
        {
            if (knockBackRoutine != null)
            {
                StopCoroutine(knockBackRoutine);
            }
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
            if (isDisabled)
                return;
            
            if (isInknockback)
                return;
            if(setter.target!=null)
                setter.target = null;
            if (!ai.pathPending && (ai.reachedEndOfPath || !ai.hasPath))
            {
                ai.destination = GetRandomPosition2D();
                ai.SearchPath();
            }
        }

        public override void ProcessKnockBack()
        {
            animator.PlayHitAnimation(interraptAttackOnDamage,() =>
            {
                ai.canMove = true;
            });
            hitEffect.Flash();
            
            if (!useKnockback)
                return;

            if (isInknockback)
                return;
            isInknockback = true;
            ai.canMove = false;

             var forceVector = currentTarget.position.x >= transform.position.x ? Vector2.left : Vector2.right;
            // var forceVector = (transform.position - currentTarget.position).normalized;
            knockBackRoutine = StartCoroutine(KnockBackRoutine(forceVector));
        }

        public override Vector2 GetVelocity()
        {
            if (!IsAggravated())
            {
                return ai.velocity.normalized;
            }

            var velocity = CurrentTarget.transform.position.x >= transform.position.x
                ? Vector2.right
                : Vector2.left;
            return velocity;
        }

        IEnumerator KnockBackRoutine(Vector2 forceVector)
        {
            yield return new WaitForEndOfFrame();
            rigidBody.AddForce(forceVector*knockbackForce,ForceMode2D.Impulse);
             yield return new WaitForSeconds(knockbackDuration);
            isInknockback = false;
            rigidBody.velocity = Vector2.zero;
        }

        Vector2 GetRandomPosition2D()
        {
            var point = Random.insideUnitCircle * wanderDistance;
            if (m_Model.EnemySpawmType == SpawnType.GroundMob)
                point.y = 0;
            point += (Vector2)ai.position;
            return point;
        }
    }
}