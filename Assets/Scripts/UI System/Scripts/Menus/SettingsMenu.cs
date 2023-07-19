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

        [SerializeField] private RectTransform _content;
        [SerializeField] private Button _backButton;

        JuicerRuntime _openEffectBG;
        JuicerRuntime _openEffectContent;

        JuicerRuntime _closeEffectBG;
        JuicerRuntime _closeEffectContent;

        public override void OnCreated()
        {
            _canvasGroup.alpha = 0;
            _content.transform.localScale = Vector3.zero;

            _openEffectBG = _canvasGroup.JuicyAlpha(1, 0.15f);
            _openEffectBG.SetOnStart(() => _canvasGroup.alpha = 0);

            _openEffectContent = _content.transform.JuicyScale(1, .15f).SetEase(Ease.EaseOutQuart).SetDelay(.05f);
            _openEffectContent.SetOnStart(() => _content.transform.localScale = Vector3.zero);

            _closeEffectContent = _content.transform.JuicyScale(0, .15f).SetEase(Ease.EaseOutQuart);

            _closeEffectBG = _canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            _closeEffectBG.SetOnComplected(CloseMenu);

            _backButton.onClick.AddListener(() =>
            {
                OnBackButtonPressed?.Invoke();
            });
        }

        public override void OnOpened()
        {
            Debug.Log("SettingsMenu opened");
            _openEffectContent.Start();
            _openEffectBG.Start();
        }

        public override void OnClosed()
        {
            _closeEffectContent.Start();
            _closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }
    }
}
