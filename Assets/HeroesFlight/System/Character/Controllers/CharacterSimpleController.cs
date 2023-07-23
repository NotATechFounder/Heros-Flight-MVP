using System;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.Character.Model;
using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterSimpleController :MonoBehaviour,CharacterControllerInterface
    {
        [SerializeField] CharacterModel model;
        CharacterMovementController m_MovementController;
        CharacterInputReceiver m_InputReceiver;
        ICharacterViewController viewController;
        Vector3 m_SavedVelocity = default;
        Transform m_Transform;

        public bool IsFacingLeft { get; private set; }
        public CharacterData Data => model.Data;
        public event Action<CharacterState> OnCharacterMoveStateChanged;
        CharacterState m_CurrentState;
        public  Vector3 GetVelocity() => m_SavedVelocity;



        void Awake()
        {
            m_MovementController = GetComponent<CharacterMovementController>();
            m_InputReceiver = GetComponent<CharacterInputReceiver>();
            viewController = GetComponent<ICharacterViewController>();
            m_Transform = GetComponent<Transform>();
            viewController.SetupView(model.Data.AppearenceModel.Data);
            m_CurrentState = CharacterState.Idle;
            IsFacingLeft = true;
        }

        void FixedUpdate()
        {
            ControllerUpdate();
        }

        void ControllerUpdate()
        {
            var input = m_InputReceiver.GetInput();
            var velocity = CalculateCharacterVelocity(input);
            m_SavedVelocity = velocity;
            m_MovementController.SetVelocity(velocity);
            UpdateCharacterState(input);
        }

        void UpdateCharacterState(Vector3 input)
        {
            var newState = CharacterState.Idle;
            if (input.Equals(Vector3.zero))
            {
                newState = CharacterState.Idle;
            }
            
            if (Mathf.Abs(input.y) < 0.4f)
            {
                switch (input.x)
                {
                    case > 0:
                        newState = CharacterState.FlyingRight;
                        break;
                    case < 0:
                        newState = CharacterState.FlyingLeft;
                        break;
                    case 0 :
                        newState = CharacterState.Idle;
                        break;
                            
                }
            }
            else
            {
                switch (input.y)
                {
                    case > 0.4f:
                        newState = CharacterState.FlyingUp;
                        break;
                    case < -0.4f:
                        newState = CharacterState.FlyingDown;
                        break;
              
                }
            }
           

           

            bool facingLeft;
            if (input.x != 0)
            {
                facingLeft = !(input.x > 0);
            }
            else
            {
                facingLeft = IsFacingLeft;
            }
            

            if (m_CurrentState == newState && IsFacingLeft==facingLeft)
                return;

            IsFacingLeft = facingLeft;
            m_CurrentState = newState;
            OnCharacterMoveStateChanged?.Invoke(m_CurrentState);
        }

        Vector3 CalculateCharacterVelocity(Vector3 inputVector)
        {
            var velocity = CalculateMovementDirection(inputVector);
            return velocity * model.Data.CombatModel.Speed;
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