using System;
namespace HeroesFlight.System.UI
{
    public interface IUISystem : ISystemInterface
    {
        event Action OnReturnToMainMenuRequest;
        event Action OnRestartLvlRequest;
        event Action OnReviveCharacterRequest;
        public UIEventHandler UiEventHandler { get; }
    }
}