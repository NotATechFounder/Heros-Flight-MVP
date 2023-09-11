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
            ai.maxSpeed =m_Model.AiData.MoveSpeed;
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

            isDisabled = true;
            setter.target = null;
            ai.canMove = false;
            ai.isStopped = true;
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
            if(isDisabled)
                return;
            
            if (isInknockback)
                return;
            
            SetMovementState(!InAttackRange());
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
            animator.PlayHitAnimation(m_Model.AttacksInteruptable,() =>
            {
                SetMovementState(true);
            });
            hitEffect.Flash();
            
            if (!m_Model.UseKnockBack)
                return;

            if (isInknockback)
                return;
            isInknockback = true;
            SetMovementState(false);

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
            rigidBody.AddForce(forceVector*m_Model.KnockBackForce,ForceMode2D.Impulse);
             yield return new WaitForSeconds(m_Model.KnockBackDuration);
            isInknockback = false;
            rigidBody.velocity = Vector2.zero;
        }

        Vector2 GetRandomPosition2D()
        {
            var point = Random.insideUnitCircle * m_Model.WanderingDistance;
            if (m_Model.EnemySpawmType == SpawnType.GroundMob)
                point.y = 0;
            point += (Vector2)ai.position;
            return point;
        }


        public override void SetMovementState(bool canMove)
        {
            base.SetMovementState(canMove);
            ai.canMove = canMove;
        }
    }
}