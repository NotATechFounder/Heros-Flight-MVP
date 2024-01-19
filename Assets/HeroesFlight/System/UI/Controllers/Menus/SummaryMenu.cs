using Pelumi.Juicer;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using HeroesFlight.System.UI.Reward;
using System.Collections.Generic;
using Plugins.Audio_System;
using System.Collections;

namespace UISystem
{
    public class SummaryMenu : BaseMenu<SummaryMenu>
    {
        public Func<string> GetCurrentGold;
        public Func<TimeSpan> GetCurrentTime;
        public Func<List<RewardVisualEntry>> GetRewardVisuals;

        public Func<int> GetAccountLevel;
        public Func<int> GetExpReward;
        public Func<int> GetCurrentExp;
        public Func<int> GetExpToNextLevel;

        public event Action OnContinueButtonClicked;

        [Header("Texts")]
        [SerializeField] TextMeshProUGUI coinText;
        [SerializeField] TextMeshProUGUI timeText;

        [Header("level")]
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI expText;
        [SerializeField] Image expBar;

        [Header("Buttons")]
        [SerializeField] AdvanceButton homeButton;
        [SerializeField] AdvanceButton retryButton;
        [SerializeField] AdvanceButton continueButton;

        [Header("Rewards")]
        [SerializeField] RewardView entryPrefab;
        [SerializeField] Transform rewardsParent;

        private List<RewardView> rewards = new List<RewardView>();
        JuicerRuntime openEffectBG;
        JuicerRuntime closeEffectBG;
        JuicerRuntimeCore<float> levelProgressEffect;

        public override void OnCreated()
        {
            openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

            closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f);
            closeEffectBG.SetOnCompleted(CloseMenu);

            continueButton.onClick.AddListener(CloseButtonAction);

            levelProgressEffect = expBar.JuicyFillAmount(1, 1f);
        }

        public override void OnOpened()
        {
            openEffectBG.Start();

            coinText.text = GetCurrentGold.Invoke();

            TimeSpan time = GetCurrentTime.Invoke();
            string timeValue = "";
            if (time.Days != 0) timeValue = String.Format("{0:D1} D : {1:D2} H: {2:D2}M : {3:D2} S", time.Days, time.Hours, time.Minutes, time.Seconds);
            else if (time.Days == 0 && time.Hours == 0 && time.Minutes == 0) timeValue = String.Format("{0:D2} S", time.Seconds);
            else if (time.Days == 0 && time.Hours == 0) timeValue = String.Format("{0:D2} M : {1:D2} S", time.Minutes, time.Seconds);
            else timeValue = String.Format("{0:D2}H: {1:D2} M : {2:D2} S", time.Hours, time.Minutes, time.Seconds);
            timeText.text = timeValue;

            List<RewardVisualEntry> rewardVisualEntries = GetRewardVisuals.Invoke();
            foreach (var reward in rewards)
            {
                Destroy(reward.gameObject);
            }

            foreach (var reward in rewardVisualEntries)
            {
                AddRewardEntry(reward);
            }
        }

        public override void OnClosed()
        {
            closeEffectBG.Start();
            foreach (Transform child in rewardsParent)
            {
                Destroy(child.gameObject);
            }
        }

        public override void ResetMenu()
        {

        }

        public void Display(int currentLevel, int numberOfLevelInc, float value, Action OnComplete)
        {
            Open();
            StartCoroutine(UpdateExpBarRoutine(currentLevel, numberOfLevelInc, value, OnComplete));
        }


        public IEnumerator UpdateExpBarRoutine(int currentLevel, int numberOfLevelInc, float value, Action OnComplete)
        {
            continueButton.gameObject.SetActive(false);

            levelText.text = "Level " + (currentLevel).ToString();

            yield return new WaitForSeconds(0.25f);

            for (int i = 0; i < numberOfLevelInc; i++)
            {
                levelProgressEffect.StartNewDestination(1f);

                AudioManager.PlaySoundEffect("LevelUp", SoundEffectCategory.UI);

                yield return new WaitUntilJuicerCompleted(levelProgressEffect);

                yield return new WaitForSeconds(0.1f);

                levelText.text = "Level " + (currentLevel + i + 1).ToString();
                expBar.fillAmount = 0;

                yield return new WaitForSeconds(0.1f);
            }

            if (value > 0)
            {

                AudioManager.PlaySoundEffect("ExperienceUp", SoundEffectCategory.UI);
                levelProgressEffect.StartNewDestination(value);
                yield return new WaitUntilJuicerCompleted(levelProgressEffect);
            }

            yield return new WaitForSeconds(.25f);

            continueButton.gameObject.SetActive(true);

            OnComplete?.Invoke();
        }

        private void CloseButtonAction()
        {
            OnContinueButtonClicked?.Invoke();
            Close();
        }

        public void AddRewardEntry(RewardVisualEntry rewardVisualEntry)
        {
            RewardView entry = Instantiate(entryPrefab, rewardsParent);
            entry.SetVisual(rewardVisualEntry);
            rewards.Add(entry);
        }
    }
}
