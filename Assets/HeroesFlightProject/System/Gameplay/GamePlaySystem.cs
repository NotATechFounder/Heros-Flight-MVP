using System;
using HeroesFlight.System.Character;
using HeroesFlight.System.UI;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Gameplay
{
    public class GamePlaySystem : IGamePlaySystem
    {
        public GamePlaySystem(IUISystem uiSystem,ICharacterSystem characterSystem)
        {
            m_UiSystem = uiSystem;
        }

        IUISystem m_UiSystem;
        public void Init(Scene scene = default, Action OnComplete = null) { }

        public void Reset() { }
    }
}