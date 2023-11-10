using Pelumi.Juicer;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    private CanvasGroup canvasGroup;
    BoosterContainer boosterContainer;

    public BoosterSO GetBoosterSO => boosterSO;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        infoBuilder = new StringBuilder();
        infoHolder.transform.localScale = new Vector3(0, 1, 1);
        infoHolderOpenEffect = infoHolder.JuicyScaleX(1, .15f);
        infoHolderCloseEffect = infoHolder.JuicyScaleX(0, .15f);
        infoHolderCloseEffect.SetOnCompleted(() => infoHolder.gameObject.SetActive(false));
        advanceButton.onClick.AddListener(() =>
        {
            ShowInfo();
        });
        canvasGroup.alpha = 0;
    }

    public void Initialize(BoosterContainer boosterContainer)
    {
        canvasGroup.alpha = 1;
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

    public void UpdateDurationBar(float duration)
    {
        durationText.text = boosterContainer.CurrentDuration.ToString("F0") + ".S";
        durationBar.value = duration;
    }

    public void Disable()
    {
        canvasGroup.alpha = 0;
        content.SetActive(false);
        boosterSO = null;
    }
}
