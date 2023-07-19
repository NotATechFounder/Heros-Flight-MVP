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

        [SerializeField] private AdvanceButton settingsButton;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AdvanceButton resumeButton;
        [SerializeField] private AdvanceButton quitButton;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            canvasGroup.alpha = 0;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            settingsButton.onClick.AddListener(() => OnSettingsButtonClicked?.Invoke());
            closeButton.onClick.AddListener(() => OnResumeButtonClicked?.Invoke());
            resumeButton.onClick.AddListener(() => OnResumeButtonClicked?.Invoke());
            quitButton.onClick.AddListener(() => OnQuitButtonClicked?.Invoke());
        }

        public override void OnOpened()
        {
            openEffectBG.Start();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }
    }
}
