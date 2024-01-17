
using System;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : SystemInterface
    {
        event Action OnLevelFailed;
        event Action OnWorldCompleted;
        void StartGameSession();

    }
}