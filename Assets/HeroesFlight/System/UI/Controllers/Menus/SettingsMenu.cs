using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.UI;

namespace UISystem
{
    public class SettingsMenu : BaseMenu<SettingsMenu>
    {
        public event Action OnBackButtonPressed;

        [SerializeField] private RectTransform content;
        [SerializeField] private Button backButton;

        [Header("Audio")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider soundEffectSlider;

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContent;

        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContent;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            openEffectContent = content.transform.JuicyScale(1, .15f).SetEase(Ease.EaseOutQuart).SetDelay(.05f);

            closeEffectContent = content.transform.JuicyScale(0, .15f).SetEase(Ease.EaseOutQuart);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            backButton.onClick.AddListener(() =>
            {
                OnBackButtonPressed?.Invoke();
            });

            musicSlider.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetMusicVolume(value);
            });

            soundEffectSlider.onValueChanged.AddListener((value) =>
            {
                AudioManager.Instance.SetSoundEffectVolume(value);
            });
        }

        public override void OnOpened()
        {
            canvasGroup.alpha = 0;
            content.transform.localScale = Vector3.zero;

            openEffectContent.Start();
            openEffectBG.Start();
        }

        public override void OnClosed()
        {
            closeEffectContent.Start();
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }
    }
}
