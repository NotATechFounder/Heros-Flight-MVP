using System;

namespace UISystem
{
    [Serializable]
    public class GameButton
    {
        public enum GameButtonType
        {
            Mainmenu_Play, Mainmenu_Pause,
            GameMenu_Pause
        }

        public enum GameButtonVisiblity
        {
            Visible,
            Hidden
        }

        public GameButtonType type;
        public AdvanceButton advanceButton;
        private GameButtonVisiblity currentState;

        public GameButtonVisiblity GetVisiblity => currentState;
        public void ToggleVisibility(GameButtonVisiblity state)
        {
            currentState = state;
            advanceButton.interactable = state == GameButtonVisiblity.Visible;
        }
    }
}
