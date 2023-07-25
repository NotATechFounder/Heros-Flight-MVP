
using HeroesFlight.Core.Application;
using HeroesFlight.System.Character;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Input;
using HeroesFlight.System.UI;
using StansAssets.Foundation.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.Core.Bootstrapper
{

    public class FakeBootStraper : MonoBehaviour
    {
        private void Start()
        {
            IUISystem uiSystem = new UiSystem();
            uiSystem.Init(SceneManager.GetActiveScene());
            uiSystem.UiEventHandler.MainMenu.Open();
        }
    }
}