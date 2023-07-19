using System;
using HeroesFlight.System.Input.Model;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Input
{
    public class InputSystem : IInputSystem
    {
        public event Action<InputModel> OnInput;

        public void Init(Scene scene = default, Action OnComplete = null) { }

        public void Reset() { }
    }
}