using HeroesFlightProject.System.NPC.Controllers;
using UnityEngine;

public class JumpMover : MonoBehaviour
{
    [SerializeField] float timeBetweenJumps=1f;
    [SerializeField] float jumpStrenght;
    Rigidbody2D rigidBody;
    float timeSinceLastJump;

    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        timeSinceLastJump = 0;
    }

    void Update()
    {
        timeSinceLastJump += Time.deltaTime;
    }

    public void Jump(Vector2 targetDirection)
    {
        if (timeSinceLastJump < timeBetweenJumps)
            return;
      
        var jumpDirection = targetDirection + Vector2.up;
       
        rigidBody.AddForce(jumpDirection * jumpStrenght);
        timeSinceLastJump = 0;


    }
}
