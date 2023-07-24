using System;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System
{
    public interface ISystemInterface
    {
        void Init(Scene scene = default, Action onComplete = null);
        void Reset();
    }
}