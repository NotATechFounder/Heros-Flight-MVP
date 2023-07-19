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

        JuicerRuntime openEffectBG;
        JuicerRuntime openEffectContent;

        JuicerRuntime closeEffectBG;
        JuicerRuntime closeEffectContent;

        public override void OnCreated()
        {
            canvasGroup.alpha = 0;
            content.transform.localScale = Vector3.zero;

            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);

            openEffectContent = content.transform.JuicyScale(1, .15f).SetEase(Ease.EaseOutQuart).SetDelay(.05f);
            openEffectContent.SetOnStart(() => content.transform.localScale = Vector3.zero);

            closeEffectContent = content.transform.JuicyScale(0, .15f).SetEase(Ease.EaseOutQuart);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            backButton.onClick.AddListener(() =>
            {
                OnBackButtonPressed?.Invoke();
            });
        }

        public override void OnOpened()
        {
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
