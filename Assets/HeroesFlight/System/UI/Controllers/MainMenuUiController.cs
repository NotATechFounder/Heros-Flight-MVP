using System;
using UnityEngine;
using UnityEngine.UI;

namespace HeroesFlight.System.UI.Controllers
{
    public class MainMenuUiController : BaseUiController,IMainMenuController
    {
        [SerializeField] Button m_PlayButton;
        public event Action OnGameSessionStartRequest;
       
        public override void Init()
        {
            base.Init();
            m_PlayButton.onClick.AddListener(() =>
            {
                OnGameSessionStartRequest.Invoke();
            });
            Hide();
        }

     
    }
}