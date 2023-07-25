using System;
using HeroesFlight.System.Input.Container;
using HeroesFlight.System.Input.Enum;
using HeroesFlight.System.Input.Model;
using StansAssets.Foundation.Extensions;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Input
{
    public class InputSystem : IInputSystem
    {
        public event Action<InputModel> OnInput;
        InputContainer container;
        public InputModel GetMovementInput()
        {
            var input = container.GetMovementInput();
            return new InputModel(InputType.Movement, new InputVectorValue(input));
        }

        public void Init(Scene scene = default, Action OnComplete = null)
        {
            container = scene.GetComponent<InputContainer>();
        }

        public void Reset() { }
    }
}