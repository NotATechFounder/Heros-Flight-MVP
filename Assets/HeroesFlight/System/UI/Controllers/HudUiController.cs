using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeroesFlight.System.UI.Controllers
{
    public class HudUiController : BaseUiController,IHudController
    {
        [SerializeField] Button m_ReturnButton;
        public event Action OnReturnToMainMenuRequest;

        public override void Init()
        {
           base.Init();
           m_ReturnButton.onClick.AddListener(() =>
           {
               OnReturnToMainMenuRequest?.Invoke();
           });
           Hide();
        }

       
    }
}