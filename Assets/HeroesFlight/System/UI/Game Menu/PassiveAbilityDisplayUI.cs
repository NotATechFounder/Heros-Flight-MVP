using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PassiveAbilityDisplayUI : MonoBehaviour
{
    [SerializeField] private float waitDuration = 1f;
    [SerializeField] private AdvanceButton advanceButton;
    [SerializeField] private GameObject content;
    [SerializeField] private RectTransform infoHolder;
    [SerializeField] private Image infoHolderIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI infoHolderText;
    [SerializeField] private TextMeshProUGUI levelText;

    private JuicerRuntime infoHolderOpenEffect;
    private JuicerRuntime infoHolderCloseEffect;
    private Coroutine viewInfoCoroutine;

    public bool Occupied => content.activeSelf;

    int level = 0;
 

    private void Awake()
    {
        infoHolder.transform.localScale = new Vector3(0, 1, 1);
        infoHolderOpenEffect = infoHolder.JuicyScaleX(1, .15f);
        infoHolderCloseEffect = infoHolder.JuicyScaleX(0, .15f);
        infoHolderCloseEffect.SetOnCompleted(() => infoHolder.gameObject.SetActive(false));
        advanceButton.onClick.AddListener(() =>
        {
            ShowInfo();
        });
        Disable();
    }

    public void Initialize(PassiveAbilityVisualData passiveAbilityVisualData, int level = 0)
    {
        infoHolderIcon.sprite = passiveAbilityVisualData.Icon;
        nameText.text = passiveAbilityVisualData.DisplayName;
        infoHolderText.text = passiveAbilityVisualData.Description;
        gameObject.SetActive(true);

        ShowInfo();
        SetLevel (level);
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
        infoHolderOpenEffect.Start(() => infoHolder.gameObject.SetActive(true));
    }

    public void CloseInfoHolder()
    {
        infoHolderCloseEffect.Start();
    }

    public void Disable()
    {
        level = 0;
        gameObject.SetActive(false);
    }

    public void SetLevel(int level)
    {
        levelText.text = $"Lvl. {level}";
    }
}
