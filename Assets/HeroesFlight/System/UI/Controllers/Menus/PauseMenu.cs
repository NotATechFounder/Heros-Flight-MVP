using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        [Header("CUrrencies")]
        [SerializeField] private TextMeshProUGUI runeShardsTMP;
        [SerializeField] private TextMeshProUGUI keysTMP;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            settingsButton.onClick.AddListener(() => OnSettingsButtonClicked?.Invoke());
            closeButton.onClick.AddListener(() => OnResumeButtonClicked?.Invoke());
            resumeButton.onClick.AddListener(() => OnResumeButtonClicked?.Invoke());
            quitButton.onClick.AddListener(() => OnQuitButtonClicked?.Invoke());
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            openEffectBG.Start();
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        public void UpdateCurrencyUi(int runeShards, int keys)
        {
            runeShardsTMP.text = runeShards.ToString();
            keysTMP.text = keys.ToString();
        }
    }
}
