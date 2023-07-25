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
        private event Action onYesButtonPressed;
        private event Action onNoButtonPressed;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Buttons")]
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AdvanceButton yesButton;
        [SerializeField] private AdvanceButton noButton;

        JuicerRuntime _openEffectBG;
        JuicerRuntime _closeEffectBG;

        public override void OnCreated()
        {
            canvasGroup.alpha = 0;

            _openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            _openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);

            _closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
            _closeEffectBG.SetOnComplected(CloseMenu);

            closeButton.onClick.AddListener(() =>
            {
                onNoButtonPressed?.Invoke();
                CloseAndReset();
            });

            yesButton.onClick.AddListener(() => 
            {
                onYesButtonPressed?.Invoke();
                CloseAndReset();
            });

            noButton.onClick.AddListener(() =>
            {
                onNoButtonPressed?.Invoke();
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
            titleText.text = "";
            questionText.text = "";
            descriptionText.text = "";
            onYesButtonPressed = null;
            onNoButtonPressed = null;
        }

        public void Display (ConfirmationUISO confirmationUISO, Action Yes, Action No, float timer = 0)
        {
            titleText.text = confirmationUISO.TitleText;
            questionText.text = confirmationUISO.QuestionText;
            descriptionText.text = confirmationUISO.DescriptionText;
            onYesButtonPressed = Yes;
            onNoButtonPressed = No;
            Open();
        }
    }
}
