using HeroesFlight.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class AiJumpController : AiControllerBase
    {
        AiMoverInterface mover;


        public override void Init(Transform player)
        {
            mover = GetComponent<AiMoverInterface>();
            rigidBody = GetComponent<Rigidbody2D>();
            rigidBody.isKinematic = true;
            viewController = GetComponent<AiViewController>();
            viewController.Init();
            currentTarget = player;
            OnInit();
            viewController.StartFadeIn(2f,Enable);
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
            rigidBody.isKinematic = false;
            gameObject.SetActive(true);
        }

        public override void Disable()
        {
           gameObject.SetActive(false);
        }

        public override void ProcessKnockBack()
        {
            var forceVector = currentTarget.position.x > transform.position.x ? Vector2.left : Vector2.right;
            rigidBody.AddForce(forceVector *knockbackForce);
        }

        public override Vector2 GetVelocity()
        {
            return rigidBody.velocity.normalized;
        }
    }
}