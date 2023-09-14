using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Character.Container;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.Input;
using HeroesFlight.System.Input.Model;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Character
{
    public class CharacterSystem : CharacterSystemInterface
    {
        public CharacterSystem(InputSystemInterface inputSystem)
        {
            inputSystem.OnInput += HandleCharacterInput;
            
        }

        public CharacterControllerInterface CurrentCharacter => characterController;

        CharacterControllerInterface characterController;
        CharacterContainer container;
        CharacterType targetCharacterType;
       


        public void Init(Scene scene = default, Action OnComplete = null)
        {
            container = scene.GetComponentInChildren<CharacterContainer>();
        }

        public void Reset()
        {
            characterController = null;
            container.Reset();
        }

        public CharacterControllerInterface CreateCharacter(Vector2 position)
        {
            characterController = container.CreateCharacter(targetCharacterType, position);
            return characterController;
        }

        public void SetCurrentCharacterType(CharacterType currentType)
        {
            targetCharacterType = currentType;
        }

        public void SetCharacterControllerState(bool isEnabled)
        {
            container.SetCharacterControllerState(isEnabled);
        }

        public void ResetCharacter(Vector2 position)
        {
            container.ResetCharacter(position);
        }

      


        void HandleCharacterInput(InputModel obj)
        {
            //process Input
        }
    }
}