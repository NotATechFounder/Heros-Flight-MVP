using System;
namespace HeroesFlight.System.UI
{
    public interface IUISystem : ISystemInterface
    {
        public UIEventHandler UiEventHandler { get; }
    }
}