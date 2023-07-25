using Pelumi.Juicer;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private CountDownTimer startTimer;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

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

            startTimer = new CountDownTimer(this);
        }

        public override void OnOpened()
        {
            countDownImage.fillAmount = 1;
            countDownText.text = countDownTime.ToString("0");
            canvasGroup.alpha = 0;
            openEffectBG.Start();
            openEffectBG.SetOnComplected(StartTimer);
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
        }

        public override void ResetMenu()
        {

        }

        public void StartTimer()
        {
            startTimer.Start(countDownTime, (current) =>
            {
                if ((int)current != (int)startTimer.GetLastTime)
                {
                    countDownTextEffect.Start();
                    countDownText.text = Mathf.CeilToInt(current).ToString();
                }

                countDownImage.fillAmount = startTimer.GetNormalizedTime;

            }, () =>
            {
                CountDownCompleteAction();
            });
        }

        private void CloseButtonAction()
        {
            OnCloseButtonClicked?.Invoke();
            startTimer.Stop();
            Close();
        }

        private void CountDownCompleteAction()
        {
            OnCountDownCompleted?.Invoke();
            Close();
        }
    }
}