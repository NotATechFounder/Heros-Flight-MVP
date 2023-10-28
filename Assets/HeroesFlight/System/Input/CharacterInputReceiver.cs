using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterInputReceiver : MonoBehaviour
    {
        CharacterInputActions.CharacterActions m_InputActions;
        Vector2 input = Vector2.zero;

        void Awake()
        {
            var inputActionMap = new CharacterInputActions();
            m_InputActions = inputActionMap.Character;
            m_InputActions.Enable();
        }


        public Vector2 GetInput() => input;

        public void SetInput(Vector2 inputValueInputValue)
        {
            input = inputValueInputValue;
        }
    }
}