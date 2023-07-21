using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Pelumi.Juicer;

namespace UISystem
{
    public class GameMenu : BaseMenu<GameMenu>
    {
        public event Action OnPauseButtonClicked;
        public event Action OnSpecialAttackButtonClicked;

        [Header("Main")]
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private TextMeshProUGUI enemyCountText;
        [SerializeField] private AdvanceButton pauseButton;

        [Header("level Progress")]
        [SerializeField] private TextMeshProUGUI levelProgressText;
        [SerializeField] private Image levelProgressFill;

        [Header("Combo Counter")]
        [SerializeField] private TextMeshProUGUI comboCounterText;

        [Header("Boss")]
        [SerializeField] private GroupImageFill bossHealthFill;

        [Header("Warning")]
        [SerializeField] private RectTransform warningPanel;

        [Header("Special Attack")]
        [SerializeField] private AdvanceButton specialAttackButton;
        [SerializeField] private Image specialAttackButtonFill;
        [SerializeField] private Image specialAttackIcon;

        JuicerRuntime openEffect;
        JuicerRuntime closeEffect;
        JuicerRuntime specialEffect;
        JuicerRuntime specialIconEffect;

        // TESTING
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                FillSpecial(specialAttackButtonFill.fillAmount += 0.25F);
            }
        }

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

            ResetMenu();
        }

        public override void OnOpened()
        {
            openEffect.Start();
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
        }

        public void UpdateCoinText(int value)
        {
            coinText.text = value.ToString();
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

        public void UpdateLevelProgressFill(float value)
        {
            levelProgressFill.fillAmount = value;
        }

        public void UpdateLevelProgress(int value, float fill)
        {
            UpdateLevelProgressText(value);
            UpdateLevelProgressFill(fill);
        }

        public void UpdateBossHealthFill(float value)
        {
            bossHealthFill.SetValue(value);
        }

        public void FillSpecial(float normalisedValue)
        {
            specialAttackButtonFill.fillAmount = normalisedValue;
            if (normalisedValue >= 1)
            {
                ToggleSpecialAttackButton(true);
            }
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
                    Debug.Log("Special Attack Button Disabled");
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
    }
}
