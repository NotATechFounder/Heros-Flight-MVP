using UnityEngine;

namespace HeroesFlight.System.Character
{
    public class CharacterInputReceiver  : MonoBehaviour
    {
        Vector3 m_Input = default;
        void Update()
        {
            m_Input.x = Input.GetAxis("Horizontal");
            m_Input.y = Input.GetAxis("Vertical");
        }

        public   Vector3 GetInput() => m_Input;
    }
}