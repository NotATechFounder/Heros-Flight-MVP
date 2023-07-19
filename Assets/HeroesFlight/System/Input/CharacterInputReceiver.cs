using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace HeroesFlight.System.Character
{
    public class CharacterInputReceiver  : MonoBehaviour
    {
        CharacterInputActions.CharacterActions m_InputActions;
       
        void Awake()
        {
            var inputActionMap= new CharacterInputActions();
            m_InputActions = inputActionMap.Character;
            m_InputActions.Enable();

        }
       
       

        public   Vector2 GetInput() =>  m_InputActions.Move.ReadValue<Vector2>();
    }
}