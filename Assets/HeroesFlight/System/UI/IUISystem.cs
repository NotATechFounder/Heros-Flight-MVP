using System;
using HeroesFlight.System.UI.Enum;

namespace HeroesFlight.System.UI
{
    public interface IUISystem : ISystem
    {
        event Action OnStartGameSessionRequest;
        event Action OnReturnToMainMenuRequest;

        void SetLoaderState(bool isEnabled);
        void SetUiState(UiSystemState newState);

      
    }
}