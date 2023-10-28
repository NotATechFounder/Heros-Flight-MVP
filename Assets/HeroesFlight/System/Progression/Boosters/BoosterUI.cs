using Pelumi.Juicer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System;

public class BoosterUI : MonoBehaviour
{
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private AdvanceButton advanceButton;
    [SerializeField] private GameObject content;
    [SerializeField] private RectTransform infoHolder;
    [SerializeField] private Image infoHolderIcon;
    [SerializeField] private TextMeshProUGUI infoHolderText;
    [SerializeField] private TextMeshProUGUI durationText;
    [SerializeField] private Slider durationBar;

    private BoosterSO boosterSO;
    private JuicerRuntime infoHolderOpenEffect;
    private JuicerRuntime infoHolderCloseEffect;
    private Coroutine viewInfoCoroutine;
    private StringBuilder infoBuilder;
    BoosterContainer boosterContainer;

    public BoosterSO GetBoosterSO => boosterSO;

    private void Awake()
    {
        infoBuilder = new StringBuilder();
        infoHolder.transform.localScale = new Vector3(0, 1, 1);
        infoHolderOpenEffect = infoHolder.JuicyScaleX(1, .15f);
        infoHolderCloseEffect = infoHolder.JuicyScaleX(0, .15f);
        infoHolderCloseEffect.SetOnCompleted(() => infoHolder.gameObject.SetActive(false));
        advanceButton.onClick.AddListener(() =>
        {
            ShowInfo();
        });
    }

    public void Initialize(BoosterContainer boosterContainer)
    {
        this.boosterContainer = boosterContainer;
        boosterContainer.OnTick = UpdateDurationBar;
        boosterContainer.OnEnd = Disable;
        boosterContainer.OnResetDuration = ShowInfo;

        boosterSO = boosterContainer.ActiveBoost.boosterSO;
        infoHolderIcon.sprite = boosterSO.BoosterSprite;
        infoBuilder.Clear();
        infoBuilder.Append(boosterSO.Abreviation);
        infoBuilder.Append(" ");
        infoBuilder.Append(boosterSO.BoosterDuration.ToString());
        infoBuilder.Append("%");

        infoHolderText.text = infoBuilder.ToString();
        content.SetActive(true);
        ShowInfo();
    }

    private void ShowInfo()
    {
        if (viewInfoCoroutine != null)
        {
            StopCoroutine(viewInfoCoroutine);
        }
        viewInfoCoroutine = StartCoroutine(ViewInfo());
    }

    public IEnumerator ViewInfo()
    {
        OpenInfoHolder();
        yield return new WaitForSeconds(waitDuration);
        CloseInfoHolder();
    }

    public void OpenInfoHolder()
    {
        infoHolderOpenEffect.Start(()=>infoHolder.gameObject.SetActive(true));
    }

    public void CloseInfoHolder()
    {
        infoHolderCloseEffect.Start();
    }

    public void UpdateDurationBar(float duration)
    {
        durationText.text = boosterContainer.CurrentDuration.ToString("F0") + ".S";
        durationBar.value = duration;
    }

    public void Disable()
    {
        content.SetActive(false);
        boosterSO = null;
    }
}
