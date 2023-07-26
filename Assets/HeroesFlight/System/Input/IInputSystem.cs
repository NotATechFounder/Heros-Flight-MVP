using System;
using HeroesFlight.System.Input.Model;

namespace HeroesFlight.System.Input
{
    public interface IInputSystem : ISystemInterface
    {
        event Action<InputModel> OnInput;
        InputModel GetMovementInput();
    }
}