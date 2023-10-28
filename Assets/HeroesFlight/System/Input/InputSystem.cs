using System;
using HeroesFlight.System.Input.Container;
using HeroesFlight.System.Input.Enum;
using HeroesFlight.System.Input.Model;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Input
{
    public class InputSystem : InputSystemInterface
    {
        public event Action<InputModel> OnInput;
        InputContainer container;
       
        public void Init(Scene scene = default, Action OnComplete = null)
        {
            container = scene.GetComponentInChildren<InputContainer>();
            container.OnMovementInput += HandleMovementInput;
        }

        private void HandleMovementInput(Vector2 input)
        {
            OnInput?.Invoke(new InputModel(InputType.Movement, new InputVectorValue(input)));
        }

        public void Reset() { }
    }
}