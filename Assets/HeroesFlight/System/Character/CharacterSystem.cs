using System;
using HeroesFlight.System.Input;
using HeroesFlight.System.Input.Model;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Character
{
    public class CharacterSystem : CharacterSystemInterface
    {

        public CharacterSystem(IInputSystem inputSystem)
        {
            inputSystem.OnInput += HandleCharacterInput;
        }

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            //implementation is your to handle
        }

        public void Reset(){ }
        public void ShakeCharacterCamera(float duration)
        {
            throw new NotImplementedException();
        }

        void HandleCharacterInput(InputModel obj)
        {
           //process Input
        }
    }
}