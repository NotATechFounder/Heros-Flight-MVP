using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using Pelumi.Juicer;
using System.ComponentModel;

namespace UISystem
{
    public class ConfirmationMenu : BaseMenu<ConfirmationMenu>
    {
        private event Action onYesButtonPressed;
        private event Action onNoButtonPressed;

        [SerializeField] private Image headerIcon;

        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI questionText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Buttons")]
        [SerializeField] private Transform container;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AdvanceButton yesButton;
        [SerializeField] private AdvanceButton noButton;

        JuicerRuntime _openEffectBG;
        JuicerRuntime openEffectContainer;
        JuicerRuntime _closeEffectBG;

        public override void OnCreated()
        {
            canvasGroup.alpha = 0;

            _openEffectBG = canvasGroup.JuicyAlpha(1, 0.25f).SetTimeMode(TimeMode.Unscaled);;
            _openEffectBG.SetOnStart(() => canvasGroup.alpha = 0);


            openEffectContainer = container.JuicyScale(Vector3.one, 0.5f)
                                            .SetEase(Ease.EaseInQuint)
                                            .SetDelay(0.25f).SetTimeMode(TimeMode.Unscaled);;

            _closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f).SetTimeMode(TimeMode.Unscaled);;
            _closeEffectBG.SetOnCompleted(CloseMenu);

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
            openEffectContainer.Start(() => container.localScale = Vector3.zero);
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

        public void Display (ConfirmationUISO confirmationUISO, Action Yes, Action No)
        {
            Display (confirmationUISO.TitleText, confirmationUISO.QuestionText, confirmationUISO.DescriptionText,  Yes, No, confirmationUISO.Icon);
        }

        public void Display(string TitleText, string QuestionText, string DescriptionText, Action Yes, Action No, Sprite Icon = null)
        {
            if (Icon != null)
            {
                headerIcon.sprite = Icon;
                headerIcon.enabled = true;
            }
            else
            {
                headerIcon.enabled = false;
            }

            titleText.text = TitleText;
            questionText.text = QuestionText;
            descriptionText.text = DescriptionText;
            onYesButtonPressed = Yes;
            onNoButtonPressed = No;
            Open();
        }
    }
}
