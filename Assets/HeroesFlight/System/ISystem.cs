using System;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System
{
    public interface ISystem
    {
        void Init(Scene scene = default, Action OnComplete = null);
        void Reset();
    }
}