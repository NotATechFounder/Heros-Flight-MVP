using System;
using UnityEngine;

namespace HeroesFlight.System.Input.Container
{
    public class InputContainer : MonoBehaviour

    {
        CharacterInputActions.CharacterActions m_InputActions;
        public event Action<Vector2> OnMovementInput; 
        void Awake()
        {
            var inputActionMap= new CharacterInputActions();
            m_InputActions = inputActionMap.Character;
            m_InputActions.Enable();

        }

        private void Update()
        {
            OnMovementInput?.Invoke(GetMovementInput());
        }


           Vector2 GetMovementInput() =>  m_InputActions.Move.ReadValue<Vector2>();
    }
}