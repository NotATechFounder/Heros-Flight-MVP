
using System;

namespace HeroesFlight.System.Gameplay
{
    public interface GamePlaySystemInterface : SystemInterface
    {
        event Action OnLevelComplected;
        event Action OnLevelFailed;
        event Action OnWorldCompleted;
        void StartGameSession();

    }
}