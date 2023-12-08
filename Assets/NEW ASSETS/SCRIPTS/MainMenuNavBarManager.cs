using Pelumi.Juicer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuNavBarManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject[] totalNavButtons;
    [SerializeField] public Color buttonDownColor = Color.gray;
    [SerializeField] public Color buttonUpColor = Color.white;

    private GameObject[] navButtonLogo;
    private TextMeshProUGUI[] navButtonText;

    private JuicerRuntime[] onNavButtonSelectMoveEffect;
    private JuicerRuntime[] onNavButtonDeselectMoveEffect;

    private JuicerRuntime[] onNavButtonSelectScaleEffect;
    private JuicerRuntime[] onNavButtonDeSelectScaleEffect;

    private void Start()
    {
        InitializeNavButtons();
        ApplyJuicerEffects(0.15f, 0.05f);
        OnNavButtonClick(2);
    }

    private void InitializeNavButtons()
    {
        navButtonLogo = new GameObject[totalNavButtons.Length];
        navButtonText = new TextMeshProUGUI[totalNavButtons.Length];

        for (int i = 0; i < navButtonLogo.Length; i++)
        {
            navButtonLogo[i] = totalNavButtons[i].transform.GetChild(0).gameObject.GetComponentInChildren<Image>().transform.parent.gameObject;
        }

        for (int i = 0; i < navButtonText.Length; i++)
        {
            navButtonText[i] = totalNavButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            navButtonText[i].gameObject.SetActive(false);
        }
    }

    private void ApplyJuicerEffects(float selectMoveDuration, float deselectMoveDuration)
    {
        onNavButtonSelectMoveEffect = new JuicerRuntime[totalNavButtons.Length];
        onNavButtonDeselectMoveEffect = new JuicerRuntime[totalNavButtons.Length];

        onNavButtonSelectScaleEffect = new JuicerRuntime[totalNavButtons.Length];
        onNavButtonDeSelectScaleEffect = new JuicerRuntime[totalNavButtons.Length];

        for (int i = 0; i < onNavButtonDeselectMoveEffect.Length; i++)
        {
            onNavButtonDeselectMoveEffect[i] = navButtonLogo[i].transform.JuicyLocalMoveY(81f, deselectMoveDuration);
            onNavButtonSelectMoveEffect[i] = navButtonLogo[i].transform.JuicyLocalMoveY((transform.position.y + 160f), selectMoveDuration);

            onNavButtonDeSelectScaleEffect[i] = navButtonLogo[i].transform.JuicyScale(Vector3.one, deselectMoveDuration);
            onNavButtonSelectScaleEffect[i] = navButtonLogo[i].transform.JuicyScale(new Vector3(1.15f, 1.15f, 1.15f), selectMoveDuration);
        }
    }

    public void OnNavButtonClick(int buttonIndex)
    {
        for (int i = 0; i < onNavButtonDeselectMoveEffect.Length; i++)
        {
            totalNavButtons[i].GetComponent<Image>().color = buttonUpColor;
            onNavButtonDeselectMoveEffect[i].Start();
            onNavButtonDeSelectScaleEffect[i].Start();
            navButtonText[i].gameObject.SetActive(false);
        }

        totalNavButtons[buttonIndex].GetComponent<Image>().color = buttonDownColor;
        onNavButtonSelectMoveEffect[buttonIndex].Start();

        navButtonLogo[buttonIndex].transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
        onNavButtonSelectScaleEffect[buttonIndex].Start();

        navButtonText[buttonIndex].gameObject.SetActive(true);
    }
}