using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class PauseMenu : BaseMenu<PauseMenu>
    {
        public event Action OnSettingsButtonClicked;
        public event Action OnResumeButtonClicked;
        public event Action OnQuitButtonClicked;

        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _quitButton;

        JuicerRuntime _openEffectBG;
        JuicerRuntime _closeEffectBG;

        public override void OnCreated()
        {
            _canvasGroup.alpha = 0;

            _openEffectBG = _canvasGroup.JuicyAlpha(1, 0.15f);
            _openEffectBG.SetOnStart(() => _canvasGroup.alpha = 0);

            _closeEffectBG = _canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            _closeEffectBG.SetOnComplected(CloseMenu);

            _settingsButton.onClick.AddListener(() => OnSettingsButtonClicked?.Invoke());
            _closeButton.onClick.AddListener(() => OnResumeButtonClicked?.Invoke());
            _resumeButton.onClick.AddListener(() => OnResumeButtonClicked?.Invoke());
            _quitButton.onClick.AddListener(() => OnQuitButtonClicked?.Invoke());
        }

        public override void OnOpened()
        {
            _openEffectBG.Start();
        }

        public override void OnClosed()
        {
            _closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }
    }
}
