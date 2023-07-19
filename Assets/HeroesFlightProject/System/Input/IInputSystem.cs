using System;
using HeroesFlight.System.Input.Model;

namespace HeroesFlight.System.Input
{
    public interface IInputSystem : ISystem
    {
        event Action<InputModel> OnInput;
    }
}