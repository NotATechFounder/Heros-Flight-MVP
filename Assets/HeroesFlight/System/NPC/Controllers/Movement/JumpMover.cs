using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

namespace HeroesFlight.System.NPC.Controllers.Movement
{
    public class JumpMover :AiBaseMovementController
    {
        [SerializeField] float timeBetweenJumps = 1f;
        [SerializeField] float jumpStrenght;
        Rigidbody2D rigidBody;
        float timeSinceLastJump;
        bool canMove;

        void Awake()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            timeSinceLastJump = 0;
            canMove = true;
        }

        void Update()
        {
            timeSinceLastJump += Time.deltaTime;
        }

        public void SetSpeed(float speed)
        {
        }

        public void SetMovementState(bool canMove)
        {
            this.canMove = canMove;
        }

        public override void MoveToTargetPosition(Vector2 targetDirection)
        {
            if (!canMove)
                return;

            if (timeSinceLastJump < timeBetweenJumps)
                return;

            var jumpDirection = targetDirection.normalized + Vector2.up;

            rigidBody.AddForce(jumpDirection * jumpStrenght);
            timeSinceLastJump = 0;
        }

        public override void MoveToRandomPosition()
        {
            var random = Random.Range(0, 2);
            var directionToMove = random == 0 ? Vector2.left : Vector2.right;
            MoveToTargetPosition(directionToMove);
        }

        public override Vector2 GetVelocity()
        {
            return rigidBody.velocity.normalized;
        }
    }
}

