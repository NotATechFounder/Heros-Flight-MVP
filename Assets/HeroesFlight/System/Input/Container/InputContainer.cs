using UnityEngine;

namespace HeroesFlight.System.Input.Container
{
    public class InputContainer : MonoBehaviour

    {
        CharacterInputActions.CharacterActions m_InputActions;
       
        void Awake()
        {
            var inputActionMap= new CharacterInputActions();
            m_InputActions = inputActionMap.Character;
            m_InputActions.Enable();

        }
       
       

        public   Vector2 GetMovementInput() =>  m_InputActions.Move.ReadValue<Vector2>();
    }
}