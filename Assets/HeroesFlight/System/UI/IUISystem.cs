using System;
namespace HeroesFlight.System.UI
{
    public interface IUISystem : ISystemInterface
    {
        event Action OnReturnToMainMenuRequest;
        public UIEventHandler UiEventHandler { get; }
    }
}