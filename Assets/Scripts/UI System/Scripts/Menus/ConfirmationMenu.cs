using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;

namespace UISystem
{
    public class ConfirmationMenu : BaseMenu<ConfirmationMenu>
    {
        private event Action _onYesButtonPressed;
        private event Action _onNoButtonPressed;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _questionText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        [Header("Buttons")]
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        JuicerRuntime _openEffectBG;
        JuicerRuntime _closeEffectBG;

        public override void OnCreated()
        {
            _canvasGroup.alpha = 0;

            _openEffectBG = _canvasGroup.JuicyAlpha(1, 0.15f);
            _openEffectBG.SetOnStart(() => _canvasGroup.alpha = 0);

            _closeEffectBG = _canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            _closeEffectBG.SetOnComplected(CloseMenu);

            _closeButton.onClick.AddListener(() =>
            {
                _onNoButtonPressed?.Invoke();
                CloseAndReset();
            });

            _yesButton.onClick.AddListener(() => 
            {
                _onYesButtonPressed?.Invoke();
                CloseAndReset();
            });

            _noButton.onClick.AddListener(() =>
            {
                _onNoButtonPressed?.Invoke();
                CloseAndReset();
            });
        }

        public override void OnOpened()
        {
            _openEffectBG.Start();
        }

        public override void OnClosed()
        {
            _closeEffectBG.Start();
        }

        private void CloseAndReset()
        {
            Close();
            ResetMenu();
        }

        public override void ResetMenu()
        {
            _titleText.text = "";
            _questionText.text = "";
            _descriptionText.text = "";
            _onYesButtonPressed = null;
            _onNoButtonPressed = null;
        }

        public void Display (string title, string question, string description, Action Yes, Action No)
        {
            _titleText.text = title;
            _questionText.text = question;
            _descriptionText.text = description;
            _onYesButtonPressed = Yes;
            _onNoButtonPressed = No;
            Open();
        }
    }
}
