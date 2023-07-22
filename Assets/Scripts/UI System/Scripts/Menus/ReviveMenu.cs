using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UISystem
{
    public class ReviveMenu : BaseMenu<ReviveMenu>
    {
        public event Action OnCloseButtonClicked;
        public event Func<bool> OnWatchAdsButtonClicked;
        public event Func<bool> OnGemButtonClicked;
        public event Action OnCountDownCompleted;

        [SerializeField] private int countDownTime = 5;
        [SerializeField] private TextMeshProUGUI countDownText;
        [SerializeField] private Image countDownImage;
        [SerializeField] private AdvanceButton closeButton;
        [SerializeField] private AdvanceButton watchAdsButton;
        [SerializeField] private AdvanceButton gemButton;
        [SerializeField] private AnimationCurve animationCurve;

        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;
        JuicerRuntime countDownTextEffect;
        private float currentTime;
        Coroutine countDownRoutine;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);
            openEffectBG.SetOnComplected(() => countDownRoutine = StartCoroutine(CountDownRoutine()));

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnComplected(CloseMenu);

            closeButton.onClick.AddListener(CloseButtonAction);

            countDownTextEffect = countDownText.transform.JuicyScale(1.5f, 0.15f);
            countDownTextEffect.SetEase(animationCurve);

            watchAdsButton.onClick.AddListener(() =>
            { 
                if (OnWatchAdsButtonClicked?.Invoke() == true)
                {
                    CloseButtonAction();
                }
            });

            gemButton.onClick.AddListener(() =>
            {
                if (OnGemButtonClicked?.Invoke() == true)
                {
                    CloseButtonAction();
                }
            });
        }

        public override void OnOpened()
        {
            countDownImage.fillAmount = 1;
            countDownText.text = countDownTime.ToString("0");
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

        private void CloseButtonAction()
        {
            OnCloseButtonClicked?.Invoke();
            StopCoroutine(countDownRoutine);
            Close();
        }

        private void CountDownCompleteAction()
        {
            OnCountDownCompleted?.Invoke();
            Close();
        }

        IEnumerator CountDownRoutine ()
        {
            currentTime = countDownTime;

            while (currentTime > 0)
            {
                float previousTime = currentTime;

                currentTime -= Time.deltaTime;

                if ((int)currentTime != (int)previousTime)
                {
                    countDownTextEffect.Start();
                    countDownText.text = currentTime.ToString("0");
                }

                countDownImage.fillAmount = currentTime / countDownTime;

                yield return null;
            }
            CountDownCompleteAction();
        }
    }
}