using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UISystem;
using UnityEngine;

public class LevelUpMenu : BaseMenu <LevelUpMenu>
{
    [SerializeField] private Transform container;

    [Header("Buttons")]
    [SerializeField] private AdvanceButton closeButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI levelText;

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
    }
}
