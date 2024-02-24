using System;
using HeroesFlight.System.Cheats.Data;

namespace HeroesFlight.System.Cheats.UI
{
    public interface CheatsUiControllerInterface
    {
        event Action<CheatButtonClickModel> OnCheatButtonClicked;
        void SetState(bool isEnabled);
        
    }
}