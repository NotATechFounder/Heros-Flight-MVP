using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
   Rigidbody2D m_RigidBody;

   void Awake()
   {
       m_RigidBody = GetComponent<Rigidbody2D>();
   }

   public void SetVelocity(Vector3 velocity)
   {
       m_RigidBody.velocity = velocity;
   }
}
