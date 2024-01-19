using HeroesFlight.System.UI.Reward;
using Pelumi.Juicer;
using Pelumi.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

public class LevelUpMenu : BaseMenu <LevelUpMenu>
{
    public event Func<RewardVisualEntry[]> GetRewardVisuals;


    [SerializeField] private Transform container;

    [Header("Buttons")]
    [SerializeField] private AdvanceButton closeButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header ("Rewards")]
    [SerializeField] private GameObject rewardViewParent;
    [SerializeField] private RewardView rewardViewPrefab;

    private List<RewardView> rewardViews = new List<RewardView>();

    JuicerRuntime openEffectBG;
    JuicerRuntime openEffectContainer;
    JuicerRuntime closeEffectBG;

    public override void OnCreated()
    {
        container.localScale = Vector3.zero;

        openEffectBG = canvasGroup.JuicyAlpha(1, 0.15f);

        openEffectContainer = container.JuicyScale(Vector3.one, 0.5f)
                                        .SetEase(Ease.EaseInQuint)
                                        .SetDelay(0.15f);

        closeEffectBG = canvasGroup.JuicyAlpha(0, 0.15f).SetDelay(.15f);
        closeEffectBG.SetOnCompleted(CloseMenu);

        closeButton.onClick.AddListener(() => Close());
    }

    public override void OnOpened()
    {
        openEffectBG.Start(() => canvasGroup.alpha = 0);
        openEffectContainer.Start(() => container.localScale = Vector3.zero);

        foreach (RewardView rewardView in rewardViews)
        {
            ObjectPoolManager.ReleaseObject(rewardView.gameObject);
        }

        rewardViews.Clear();
    }

    public override void OnClosed()
    {
        closeEffectBG.Start();
        ResetMenu();
    }

    public override void ResetMenu()
    {

    }

    public void AccountLevelUp(LevelSystem.ExpIncreaseResponse response)
    {
        if (response.numberOfLevelsGained == 0) return;
        levelText.text = response.currentLevel.ToString();
        Open();
        DisplayRewards(GetRewardVisuals?.Invoke());
    }

    public void DisplayRewards(params RewardVisualEntry[] rewardVisual)
    {
        for (int i = 0; i < rewardVisual.Length; i++)
        {
            RewardView rewardView = ObjectPoolManager.SpawnObject(rewardViewPrefab, rewardViewParent.transform, PoolType.UI);
            rewardView.SetVisual(rewardVisual[i]);
            rewardViews.Add(rewardView);
        }
    }

}
