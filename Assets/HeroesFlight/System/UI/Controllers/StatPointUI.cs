using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Plugins.Audio_System;
using Pelumi.Juicer;
using HeroesFlight.Common.Progression;
using System.Collections.Generic;
using System.Linq;

public class StatPointUI : MonoBehaviour
{
    public event Func<StatAttributeType, int> GetCurrentSpLevel;
    public event Func<StatAttributeType, bool> OnAddSpClicked;
    public event Func<StatAttributeType, bool> OnRemoveSpClicked;

    public event Action OnSpChanged;

    [SerializeField] private AdvanceButton upButton;
    [SerializeField] private AdvanceButton downButton;
    [SerializeField] private AdvanceButton diceButton;
    [SerializeField] private TextMeshProUGUI attributeName;
    [SerializeField] private TextMeshProUGUI attributeIncrement;
    [SerializeField] private TextMeshProUGUI attributeLevel;
    [SerializeField] private Image icon;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image levelUpFill;
    [SerializeField] private StatValueUI statValueUIPrefab;
    [SerializeField] private Transform statPointContainer;

    private StatPointSO statPointSO;
    private int initialSpLevel;
    private int currentSpLevel;
    private Dictionary <StatType, StatValueUI> statTypePerSp = new Dictionary<StatType, StatValueUI>();

    public StatPointSO StatPointSO => statPointSO;

    JuicerRuntime levelUpEffect;

    private void Awake()
    {
        upButton.onClick.AddListener(OnUpButtonClicked);
        downButton.onClick.AddListener(OnDownButtonClicked);
        levelUpEffect = levelUpFill.JuicyFillAmount(1, 0.15f);
    }

    public void Init(StatPointSO statPointSO)
    {
        this.statPointSO = statPointSO;
        SetAttribute();
    }

    public void SetAttribute()
    {
        icon.sprite = statPointSO.Icon;
        attributeName.text = statPointSO.StatAttributeType.ToString();

        foreach (StatPointInfo statPointInfo in statPointSO.KeyValues)
        {
            StatValueUI statValueUI = Instantiate(statValueUIPrefab, statPointContainer);
            statValueUI.Init(statPointInfo.statType, statPointInfo.valuePerSp.ToString());
            statTypePerSp.Add(statPointInfo.statType, statValueUI);
        }
    }

    public void OnUpButtonClicked()
    {
        if(OnAddSpClicked?.Invoke(statPointSO.StatAttributeType) ?? false)
        {
            OnSpChanged?.Invoke();
            currentSpLevel++;
            attributeIncrement.text = "+" + (currentSpLevel - initialSpLevel).ToString();

            UpdateNextStatValues();
        }
    }

    public void OnDownButtonClicked()
    {
        if(OnRemoveSpClicked?.Invoke(statPointSO.StatAttributeType) ?? false)
        {
            OnSpChanged?.Invoke();

            currentSpLevel--;

            if (currentSpLevel == initialSpLevel)
            {
                attributeIncrement.text = "";
            }
            else
            {
                attributeIncrement.text = "+" + (currentSpLevel - initialSpLevel).ToString();
            }

            UpdateNextStatValues();
        }
    }

    public void LoadCurrentValues()
    {
        currentSpLevel = initialSpLevel = GetCurrentSpLevel?.Invoke(statPointSO.StatAttributeType) ?? 0;
        attributeLevel.text = "LV. " + currentSpLevel.ToString();
        attributeIncrement.text = "";

        UpdateCurrentValue();
        ToggleInteractivity (true);
    }

    public void ToggleInteractivity(bool state)
    {
        upButton.interactable = state;
        downButton.interactable = state;
        diceButton.interactable = state;
    }

    public void UpdateCurrentValue()
    {
        foreach (KeyValuePair<StatType, StatValueUI> statValueUI in statTypePerSp)
        {
            statValueUI.Value.UpdateCurrentValue((currentSpLevel * statPointSO.KeyValues.FirstOrDefault(x => x.statType == statValueUI.Key).valuePerSp).ToString());
        }
    }

    public void UpdateNextStatValues()
    {
        foreach (KeyValuePair<StatType, StatValueUI> statValueUI in statTypePerSp)
        {
            if (currentSpLevel == initialSpLevel)
            {
                statValueUI.Value.UpdateNextValue(0);
            }
            else
            {
                statValueUI.Value.UpdateNextValue(currentSpLevel * statPointSO.KeyValues.FirstOrDefault(x => x.statType == statValueUI.Key).valuePerSp);
            }
        }
    }

    public bool TryUpgradeCurrentValue()
    {
        if (currentSpLevel == initialSpLevel)
        {
            return false;
        }
        OnLevelUp(currentSpLevel);
        return true;    
    }

    public void OnLevelUp(int newLevel)
    {
        ToggleInteractivity(false);

        levelUpEffect.SetOnCompleted(() =>
        {
            attributeLevel.text = "LV. " + newLevel.ToString();

            attributeIncrement.text = "";

            foreach (KeyValuePair<StatType, StatValueUI> statValueUI in statTypePerSp)
            {
                statValueUI.Value.ConfrimNewValues();
            }

        });

        AudioClip clip = AudioManager.GetSoundEffectClip("HeroProgressionGain");
        levelUpEffect.ChangeDuration(clip.length - 1.5f);
        AudioManager.PlaySoundEffect("HeroProgressionGain", SoundEffectCategory.UI);
        levelUpEffect.Start(() => levelUpFill.fillAmount = 0);
    }

    public void OnModified(bool modified)
    {
        attributeLevel.color = modified ? Color.green : Color.white;
    }


    public void ResetSpText()
    {
        attributeLevel.text = "LV. 0";
    }

    public void ToggleButtonActive(bool active)
    {
        downButton.interactable = active;
        buttonImage.color = active ? Color.white : Color.grey;
    }
}
