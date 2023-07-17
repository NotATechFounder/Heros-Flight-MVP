using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
   [SerializeField] Rigidbody2D m_RigidBody;
 
   public void SetVelocity(Vector3 velocity)
   {
       m_RigidBody.velocity = velocity;
   }
}
