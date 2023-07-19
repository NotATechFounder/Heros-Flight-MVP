using System;

namespace HeroesFlight.System.UI.Controllers
{
    public interface IHudController : IUiController
    {
        event Action OnReturnToMainMenuRequest;
    }
}