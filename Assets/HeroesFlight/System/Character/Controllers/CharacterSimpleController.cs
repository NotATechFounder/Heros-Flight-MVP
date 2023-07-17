using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterSimpleController : Controller
    {
        [SerializeField] float m_MovementSpeed = 5f;
        CharacterMovementController m_MovementController;
        CharacterInputReceiver m_InputReceiver;
        Vector3 m_SavedVelocity = default;
        Transform m_Transform;

        void Awake()
        {
            m_MovementController = GetComponent<CharacterMovementController>();
            m_InputReceiver = GetComponent<CharacterInputReceiver>();
            m_Transform = GetComponent<Transform>();
        }

        void FixedUpdate()
        {
            ControllerUpdate();
        }

        public override Vector3 GetVelocity() => m_SavedVelocity;

        void ControllerUpdate()
        {
            var input = m_InputReceiver.GetInput();
            var velocity = CalculateCharacterVelocity(input);
            m_SavedVelocity = velocity;
            m_MovementController.SetVelocity(velocity);
        }

        Vector3 CalculateCharacterVelocity(Vector3 inputVector)
        {
            var velocity = CalculateMovementDirection(inputVector);
            return velocity *= m_MovementSpeed;
        }

        Vector3 CalculateMovementDirection(Vector3 inputVector)
        {
            var velocity = Vector3.zero;
            velocity += m_Transform.right * inputVector.x;
            velocity += m_Transform.up * inputVector.y;
            
            if(velocity.magnitude>1)
                velocity.Normalize();

            return velocity;
        }
    }
}