namespace HeroesFlight.System.UI.Controllers
{
    public interface IUiLoaderController : IUiController
    {
        void UpdateLoader(float progress,int count);
    }
}