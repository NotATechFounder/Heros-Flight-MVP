using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

public class JumpMover : MonoBehaviour, AiMoverInterface
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

    public void SetSpeed(float speed) { }

    public void SetMovementState(bool canMove)
    {
        this.canMove = canMove;
    }

    public void Move(Vector2 targetDirection)
    {
        if (!canMove)
            return;

        if (timeSinceLastJump < timeBetweenJumps)
            return;

        var jumpDirection = targetDirection.normalized + Vector2.up;

        rigidBody.AddForce(jumpDirection * jumpStrenght);
        timeSinceLastJump = 0;
    }

   
}