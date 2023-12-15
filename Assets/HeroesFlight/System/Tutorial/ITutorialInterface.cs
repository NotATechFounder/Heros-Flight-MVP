using HeroesFlight.System;
using System;

namespace HeroesFlight.System.Tutorial
{
    public enum TutorialState
    {
        Gameplay,
        MainMenu,
    }

    public interface ITutorialInterface : SystemInterface
    {
        public event Action<TutorialState> OnTutorialStateChanged;
        public event Action OnFullTutorialCompleted;
    }
}
