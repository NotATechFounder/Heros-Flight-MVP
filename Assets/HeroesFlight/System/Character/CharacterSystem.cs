using System;
using HeroesFlight.System.Character.Container;
using HeroesFlight.System.Input;
using HeroesFlight.System.Input.Model;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Character
{
    public class CharacterSystem : CharacterSystemInterface
    {

        public CharacterSystem(IInputSystem inputSystem)
        {
            inputSystem.OnInput += HandleCharacterInput;
        }

        public CharacterControllerInterface CurrentCharacter => characterController;
         
        CharacterControllerInterface characterController;
        CharacterContainer container;


        public void Init(Scene scene = default, Action OnComplete = null)
        {
            container=scene.GetComponentInChildren<CharacterContainer>();
        }

        public void Reset()
        {
            characterController = null;
            container.Reset();
        }

        public CharacterControllerInterface CreateCharacter()
        {
            characterController = container.CreateCharacter();
            return characterController;
        }

        public void SetCharacterControllerState(bool isEnabled)
        {
            container.SetCharacterControllerState(isEnabled);
        }

        public void ResetCharacter()
        {
            container.ResetCharacter();
        }

       

        void HandleCharacterInput(InputModel obj)
        {
           //process Input
        }
    }
}