using HeroesFlight.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiJumpController : AiControllerBase
    {
        AiMoverInterface mover;


        public override void Init(Transform player, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon)
        {
            statModifier = monsterStatModifier;
            mover = GetComponent<AiMoverInterface>();
            rigidBody = GetComponent<Rigidbody2D>();
            attackCollider = GetComponent<Collider2D>();
            animator = GetComponent<AiAnimatorInterface>();
            rigidBody.isKinematic = true;
            viewController = GetComponent<AiViewController>();
            viewController.Init();
            currentTarget = player;
            OnInit();
            viewController.StartFadeIn(2f,Enable);
            DisplayModifiyer(currentCardIcon);
        }


        public override void ProcessFollowingState()
        {
            var directionToTarget = (Vector2)(CurrentTarget.position - transform.position);
            mover.Move(directionToTarget);
        }

        public override void ProcessWanderingState()
        {
            var random = Random.Range(0, 2);
            var directionToMove = random == 0 ? Vector2.left : Vector2.right;
            mover.Move(directionToMove);
        }

        public override void Enable()
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            gameObject.SetActive(true);
        }

        public override void Disable()
        {
            rigidBody.bodyType = RigidbodyType2D.Static;
            base.Disable();
        }

        public override void ProcessKnockBack()
        {
            rigidBody.velocity = Vector2.zero;
            var forceVector = currentTarget.position.x > transform.position.x ? Vector2.left : Vector2.right;
            forceVector.Normalize();
            rigidBody.AddForce(forceVector *knockbackForce,ForceMode2D.Impulse);
        }

        public override Vector2 GetVelocity()
        {
            return rigidBody.velocity.normalized;
        }
    }
}