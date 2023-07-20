using System;
using HeroesFlight.System.UI.Enum;

namespace HeroesFlight.System.UI
{
    public interface IUISystem : ISystemInterface
    {
        event Action OnStartGameSessionRequest;
        event Action OnReturnToMainMenuRequest;

        void SetLoaderState(bool isEnabled);
        void SetUiState(UiSystemState newState);

      
    }
}