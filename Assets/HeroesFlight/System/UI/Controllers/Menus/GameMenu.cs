using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Pelumi.Juicer;
using System.Collections;

namespace UISystem
{
    public class GameMenu : BaseMenu<GameMenu>
    {
     //   public Func<float> GetCoinText;

        public event Action OnPauseButtonClicked;
        public event Action OnSpecialAttackButtonClicked;
        public event Action<int> OnLevelUpComplete;

        [Header("Main")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI enemyCountText;
        [SerializeField] private AdvanceButton pauseButton;

        [Header("level Progress")]
        [SerializeField] private GameObject levelProgressPanel;
        [SerializeField] private TextMeshProUGUI levelProgressText;
        [SerializeField] private Image levelProgressFill;

        [Header("Combo Counter")]
        [SerializeField] private TextMeshProUGUI comboCounterText;

        [Header("Boss")]
        [SerializeField] private GroupImageFill bossHealthFill;

        [SerializeField] Canvas bossCanvas;

        [Header("Warning")]
        [SerializeField] private RectTransform warningPanel;

        [Header("Special Attack")]
        [SerializeField] private AdvanceButton specialAttackButton;
        [SerializeField] private Image specialAttackButtonFill;
        [SerializeField] private Image specialAttackIcon;

        [Header("Boosters")]
        [SerializeField] private BoosterUI[] boosterButtons;
        
        JuicerRuntime openEffect;
        JuicerRuntime closeEffect;
        JuicerRuntime specialEffect;
        JuicerRuntime specialIconEffect;
        JuicerRuntime levelProgressEffect;

        private bool isExpComplete;

        public bool IsExpComplete => isExpComplete;

        public override void OnCreated()
        {
            openEffect = canvasGroup.JuicyAlpha(1, 0.5f);
            openEffect.SetOnStart(() => canvasGroup.alpha = 0);

            closeEffect = canvasGroup.JuicyAlpha(0, 0.5f);
            closeEffect.SetOnStart(() => canvasGroup.alpha = 1);
            closeEffect.SetOnComplected(CloseMenu);

            specialEffect = specialAttackButtonFill.JuicyAlpha(0, 0.25f);
            specialEffect.SetEase(Ease.EaseInBounce);
            specialEffect.SetLoop( -1);

            specialIconEffect = specialAttackIcon.transform.JuicyScale(5f, 0.25f);
            specialIconEffect.SetOnComplected(() => ToggleSpecialAttackButton(false));

            pauseButton.onClick.AddListener(() => OnPauseButtonClicked?.Invoke());

            specialAttackButton.onClick.AddListener(SpecialAttackButtonClicked);

            levelProgressEffect = levelProgressFill.JuicyFillAmount(1, 1f);

            ResetMenu();
        }

        public override void OnOpened()
        {
            ResetMenu();
            openEffect.Start();

           // UpdateCoinText(GetCoinText());
        }

        public override void OnClosed()
        {
            closeEffect.Start();
        }

        public override void ResetMenu()
        {
            coinText.text = "0";
            timerText.text = "00:00";
            enemyCountText.text = "0";
            comboCounterText.text = "0";
            levelProgressText.text = "LV.0";
            
            foreach (BoosterUI boosterButton in boosterButtons)
            {
                if (boosterButton.GetBoosterSO != null)
                {
                    boosterButton.Disable();
                }
            }
        }

        public void UpdateCoinText(float value)
        {
            coinText.JuicyTextNumber(value, 0.5f).Start();
        }

        public void UpdateTimerText(float value)
        {
            float minutes = Mathf.FloorToInt(value / 60);
            float seconds = Mathf.FloorToInt(value % 60);
            //timerText.text = (minutes == 0) ? $"{seconds.ToString("F0")}s" : $"{minutes:00}m : {seconds:00}s";
            timerText.text = (minutes == 0) ? $"{seconds.ToString("F0")}" : $"{minutes:00} : {seconds:00}";
        }

        public void UpdateTimerText(string value)
        {
            timerText.text = value;
        }

        public void UpdateEnemyCountText(int value)
        {
            enemyCountText.text = value.ToString();
        }

        public void UpdateComboCounterText(int value)
        {
            comboCounterText.text = value.ToString();
        }

        public void UpdateLevelProgressText(int value)
        {
            levelProgressText.text = value.ToString();
        }

        public void UpdateExpBar(float value)
        {
            isExpComplete = false;
            StartCoroutine(UpdateExpBarRoutine(value));
        }

        public IEnumerator UpdateExpBarRoutine(float value)
        {
            levelProgressPanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);

            levelProgressEffect.ChangeDesination(value);
            levelProgressEffect.Start();
            yield return new WaitUntilJuicerComplected(levelProgressEffect);

            yield return new WaitForSeconds(0.25f);
            levelProgressPanel.SetActive(false);
            isExpComplete = true;
        }

        public void UpdateExpBarLevelUp(int currentLevel, int numberOfLevelInc, float value)
        {
            isExpComplete = false;
            StartCoroutine(UpdateExpBarLevelUpRoutine(currentLevel, numberOfLevelInc, value));
        }

        public IEnumerator UpdateExpBarLevelUpRoutine(int currentLevel, int numberOfLevelInc, float value)
        {
            levelProgressPanel.SetActive(true);
            yield return new WaitForSeconds(0.5f);


            for (int i = 0; i < numberOfLevelInc; i++)
            {
                levelProgressEffect.ChangeDesination(1f);
                levelProgressEffect.Start();

                yield return new WaitUntilJuicerComplected(levelProgressEffect);

                yield return new WaitForSeconds(0.1f);

                levelProgressText.text = "LV." + (currentLevel + i + 1).ToString();
                levelProgressFill.fillAmount = 0;

                yield return new WaitForSeconds(0.1f);
            }

            if (value > 0)
            {
                levelProgressEffect.ChangeDesination(value);
                levelProgressEffect.Start();
                yield return new WaitUntilJuicerComplected(levelProgressEffect);
            }

            yield return new WaitForSeconds(0.25f);
            levelProgressPanel.SetActive(false);
            OnLevelUpComplete?.Invoke(currentLevel + numberOfLevelInc);
            isExpComplete = true;
        }

        public void UpdateBossHealthFill(float value)
        {
            bossHealthFill.SetValue(value);
        }

        public void ToggleBossHpBar(bool isEnabled)
        {
            bossCanvas.enabled=isEnabled;
        }

        public void FillSpecial(float normalisedValue)
        {
            specialAttackButtonFill.fillAmount = normalisedValue;
            ToggleSpecialAttackButton(normalisedValue >= 1);
        }

        public void ToggleSpecialAttackButton(bool value)
        {
            switch (value)
            {
                case true:
                    specialAttackButtonFill.color = new Color(specialAttackButtonFill.color.r, specialAttackButtonFill.color.g, specialAttackButtonFill.color.b, 1);
                    specialEffect.Start();
                    break;
                case false:
                    specialEffect.Pause();
                    specialAttackButtonFill.color = new Color(specialAttackButtonFill.color.r, specialAttackButtonFill.color.g, specialAttackButtonFill.color.b, 1);
                    specialAttackIcon.transform.localScale = Vector3.one;
                    break;
            }
        }

        private void SpecialAttackButtonClicked()
        {
            if (specialAttackButtonFill.fillAmount < 1) return;
            
            specialAttackButtonFill.fillAmount = 0;
            specialIconEffect.Start();
            OnSpecialAttackButtonClicked?.Invoke();
        }

        public void VisualiseBooster(BoosterContainer boosterContainer)
        {
            foreach (BoosterUI boosterButton in boosterButtons)
            {
                if (boosterButton.GetBoosterSO == null)
                {
                    boosterButton.Initialize(boosterContainer);
                    break;
                }
            }
        }
    }
}
