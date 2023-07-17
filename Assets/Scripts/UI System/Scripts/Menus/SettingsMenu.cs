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

        JuicerRuntime _openEffect;
        JuicerRuntime _closeEffect;

        public override void OnCreated()
        {
            _openEffect = _content.transform.JuicyLocalMoveY(0, .5f).SetEase(Ease.EaseInExpo);
            _openEffect.SetOnStart(() => _content.transform.localPosition = new Vector3(0, -Screen.height));

            _closeEffect = _content.transform.JuicyLocalMoveY(-Screen.height, .5f).SetEase(Ease.Linear);
            _closeEffect.SetOnComplected(CloseMenu);

            _backButton.onClick.AddListener(() =>
            {
                OnBackButtonPressed?.Invoke();
            });
        }

        public override void OnOpened()
        {
            _openEffect.Start();
        }

        public override void OnClosed()
        {
            _closeEffect.Start();
        }
    }
}
