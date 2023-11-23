using System.Collections;
using System.Collections.Generic;
using HeroesFlightProject.System.Gameplay.Controllers;
using HeroesFlightProject.System.NPC.State;
using HeroesFlightProject.System.NPC.State.AIStates;
using Pathfinding;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiControllerPathFinding : AiControllerBase
    {
        AIDestinationSetter setter;
        Coroutine knockBackRoutine;
        IAstarAI ai;

        public override void Init(Transform player, int health, float damage, MonsterStatModifier monsterStatModifier,
            Sprite currentCardIcon)
        {
            setter = GetComponent<AIDestinationSetter>();
            attackCollider = GetComponent<Collider2D>();
            ai = GetComponent<IAstarAI>();
            stateMachine = new FSMachine();
            var animator = GetComponent<AiAnimationController>();
            stateMachine.AddStates(new List<FSMState>()
            {
                new AiWanderingState(this, animator, stateMachine),
                new AiChaseState(this, animator, stateMachine),
                new AiAttackState(this, animator, stateMachine),
                new AiDeathState(this, animator, stateMachine)
            });
            base.Init(player, health, damage, monsterStatModifier, currentCardIcon);
            stateMachine.SetState(typeof(AiWanderingState));
        }

        public override void Enable()
        {
            base.Enable();
            SetMovementState(true);
        }

        public override void Disable()
        {
            if (knockBackRoutine != null)
            {
                StopCoroutine(knockBackRoutine);
            }


            setter.target = null;
            SetMovementState(false);
            ai.SetPath(null);
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

        public override void ProcessKnockBack()
        {
            
            if (!m_Model.UseKnockBack)
            {
                animator.PlayHitAnimation(m_Model.AttacksInteruptable);
                hitEffect.Flash();
            }
            else
            {
                animator.PlayHitAnimation(m_Model.AttacksInteruptable, () =>
                {
                 // SetMovementState(!isDisabled);
                });
                hitEffect.Flash();


              

                var forceVector = currentTarget.position.x >= transform.position.x ? Vector2.left : Vector2.right;
                mover.ProcessKnockBack(forceVector, m_Model.KnockBackForce,m_Model.KnockBackDuration);
                // var forceVector = (transform.position - currentTarget.position).normalized;
                knockBackRoutine = StartCoroutine(KnockBackRoutine(forceVector));
            }
        }

        protected override Vector2 GetVelocity()
        {
            if (IsAggravated() && attacker.CanAttack())
            {
                var velocity = CurrentTarget.transform.position.x >= transform.position.x
                    ? Vector2.right
                    : Vector2.left;
                return velocity;
            }

            return mover.GetVelocity().normalized;
        }

        protected override void HandleDeath(IHealthController obj)
        {
            stateMachine.SetState(typeof(AiDeathState));
        }

        IEnumerator KnockBackRoutine(Vector2 forceVector)
        {
            yield return new WaitForEndOfFrame();
            rigidBody.AddForce(forceVector * m_Model.KnockBackForce, ForceMode2D.Impulse);
            yield return new WaitForSeconds(m_Model.KnockBackDuration);
            isInknockback = false;
            rigidBody.velocity = Vector2.zero;
        }
    }
}