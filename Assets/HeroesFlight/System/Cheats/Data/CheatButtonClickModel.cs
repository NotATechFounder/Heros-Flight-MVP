using HeroesFlight.System.Cheats.Enum;

namespace HeroesFlight.System.Cheats.Data
{
    public class CheatButtonClickModel
    {
        public CheatButtonClickModel(CheatsButtonType buttonType)
        {
            ButtonType = buttonType;
            ToggleValue = false;
        }
        public CheatButtonClickModel(CheatsButtonType buttonType, bool toggleValue)
        {
            ButtonType = buttonType;
            ToggleValue = toggleValue;
        }

        public CheatsButtonType ButtonType { get; }
        public bool ToggleValue { get; }
    }
}