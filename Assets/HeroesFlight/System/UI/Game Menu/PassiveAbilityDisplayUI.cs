using Pelumi.Juicer;
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

    private JuicerRuntime infoHolderOpenEffect;
    private JuicerRuntime infoHolderCloseEffect;
    private Coroutine viewInfoCoroutine;

    public bool Occupied => content.activeSelf;
 

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
    }

    public void Initialize(PassiveAbilityVisualData passiveAbilityVisualData)
    {
        infoHolderIcon.sprite = passiveAbilityVisualData.Icon;
        nameText.text = passiveAbilityVisualData.DisplayName;
        infoHolderText.text = passiveAbilityVisualData.Description;
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
        infoHolderOpenEffect.Start(() => infoHolder.gameObject.SetActive(true));
    }

    public void CloseInfoHolder()
    {
        infoHolderCloseEffect.Start();
    }

    public void Disable()
    {
        content.SetActive(false);
    }
}
