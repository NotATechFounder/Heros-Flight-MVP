using System;
using HeroesFlight.System.Input.Model;

namespace HeroesFlight.System.Input
{
    public interface InputSystemInterface : SystemInterface
    {
        event Action<InputModel> OnInput;
      
    }
}