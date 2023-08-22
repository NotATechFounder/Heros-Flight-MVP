using System;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System
{
    public interface SystemInterface
    {
        void Init(Scene scene = default, Action onComplete = null);
        void Reset();
    }
}