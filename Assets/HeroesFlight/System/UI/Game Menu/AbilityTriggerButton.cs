using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTriggerButton : MonoBehaviour
{
    public event System.Action<int> OnAbilityButtonClicked;

    [SerializeField] private Image icon;
    [SerializeField] private Image fill;
    [SerializeField] int abilityIndex;

    private JuicerRuntime effect;
    private CanvasGroup canvasGroup;
    private AdvanceButton advanceButton;

    private void Awake()
    {
        advanceButton = GetComponent<AdvanceButton>();
        canvasGroup = GetComponent<CanvasGroup>();

        effect = fill.JuicyAlpha(0, 0.25f);
        effect.SetEase(Ease.EaseInBounce);
        effect.SetLoop(-1);
        advanceButton.onClick.AddListener(OnButtonClicked);

        canvasGroup.alpha = 0;
    }

    public void UpdateSkillOneFill(float normalisedValue)
    {
        fill.fillAmount = 1 - normalisedValue;

        if (!effect.IsPaused)
        {
            effect.Pause();
            fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, 1);
        }
    }

    public void UpdateSkillOneFillCoolDown(float normalisedValue)
    {
        fill.fillAmount = normalisedValue;

        if (fill.fillAmount >= 1)
        {
            fill.color = new Color(fill.color.r, fill.color.g, fill.color.b, 1);
            effect.Start();
        }
    }

    public void OnButtonClicked()
    {
        if (fill.fillAmount != 0)
            return;
        OnAbilityButtonClicked?.Invoke(abilityIndex);
    }

    public void SetIcon(Sprite icon)
    {
        this.icon.sprite = icon;
        canvasGroup.alpha = .9f;
    }

    public void Disable()
    {
      icon.sprite = null;
        canvasGroup.alpha = 0;
    }
}
