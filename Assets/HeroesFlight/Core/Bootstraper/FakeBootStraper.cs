
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
        private void Awake()
        {
            //GamePlaySystemInterface gameplay = new GamePlaySystem(null,null);
            //IUISystem uiSystem = new UiSystem(gameplay);
            //uiSystem.Init(SceneManager.GetActiveScene());

            UIEventHandler uiEventHandler = FindAnyObjectByType<UIEventHandler>();
            uiEventHandler.Init();
        }
    }
}