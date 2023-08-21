using System;
namespace HeroesFlight.System.UI
{
    public interface IUISystem : SystemInterface
    {
        event Action OnReturnToMainMenuRequest;
        event Action OnRestartLvlRequest;
        event Action OnReviveCharacterRequest;
        event Action OnSpecialButtonClicked;
        public UIEventHandler UiEventHandler { get; }
    }
}