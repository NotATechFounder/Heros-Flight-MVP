using System;

namespace HeroesFlight.System.UI.Controllers
{
    public interface IMainMenuController : IUiController
    {
        event Action OnGameSessionStartRequest;
    }
}