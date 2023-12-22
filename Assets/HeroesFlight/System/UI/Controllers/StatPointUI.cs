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
    public event Func <StatAttributeType, int> GetDiceRollValue;
    public event Func<StatAttributeType, int> GetCurrentSpLevel;
    public event Func<StatAttributeType, bool> OnAddSpClicked;
    public event Func<StatAttributeType, bool> OnRemoveSpClicked;

    public event Action OnSpChanged;
    public event Action<StatAttributeType, int> OnDiceClicked;

    [SerializeField] private AdvanceButton upButton;
    [SerializeField] private AdvanceButton downButton;
    [SerializeField] private AdvanceButton diceButton;
    [SerializeField] private TextMeshProUGUI attributeName;
    [SerializeField] private TextMeshProUGUI attributeIncrement;
    [SerializeField] private TextMeshProUGUI attributeLevel;
    [SerializeField] private TextMeshProUGUI attributeDiceRoll;
    [SerializeField] private Image icon;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image levelUpFill;
    [SerializeField] private StatValueUI statValueUIPrefab;
    [SerializeField] private Transform statPointContainer;

    private StatPointSO statPointSO;
    private int initialSpLevel;
    private int currentSpLevel;
    private int currentDiceRollValue;
    private int spIncrementValue;
    private Dictionary <StatType, StatValueUI> statTypePerSp = new Dictionary<StatType, StatValueUI>();

    public StatPointSO StatPointSO => statPointSO;
    public AdvanceButton UpButton => upButton;
    public AdvanceButton DownButton => downButton;
    public AdvanceButton DiceButton => diceButton;

    JuicerRuntime levelUpEffect;

    private void Awake()
    {
        upButton.onClick.AddListener(OnUpButtonClicked);
        downButton.onClick.AddListener(OnDownButtonClicked);
        diceButton.onClick.AddListener(() =>
        {
            OnDiceClicked?.Invoke(statPointSO.StatAttributeType, GetDiceRollValue?.Invoke(statPointSO.StatAttributeType) ?? 0);
        });
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
            spIncrementValue++;
            attributeIncrement.text = "+" + spIncrementValue.ToString();

            UpdateNextStatValues();
        }
    }

    public void OnDownButtonClicked()
    {
        if(OnRemoveSpClicked?.Invoke(statPointSO.StatAttributeType) ?? false)
        {
            OnSpChanged?.Invoke();
            spIncrementValue--;

            if (spIncrementValue == 0)
            {
                attributeIncrement.text = "";
            }
            else
            {
                attributeIncrement.text = "+" + spIncrementValue.ToString();
            }

            UpdateNextStatValues();
        }
    }

    public void LoadCurrentValues()
    {
        currentSpLevel = initialSpLevel = GetCurrentSpLevel.Invoke(statPointSO.StatAttributeType);
        currentDiceRollValue = GetDiceRollValue.Invoke(statPointSO.StatAttributeType);
        attributeLevel.text = "LV. " + currentSpLevel.ToString();
        attributeIncrement.text = "";

        UpdateCurrentValue();
        SetDiceRoll(GetDiceRollValue.Invoke(statPointSO.StatAttributeType));
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
            statValueUI.Value.UpdateCurrentValue(((currentSpLevel + currentDiceRollValue) * statPointSO.KeyValues.FirstOrDefault(x => x.statType == statValueUI.Key).valuePerSp).ToString());
        }
    }

    public void UpdateNextStatValues()
    {
        foreach (KeyValuePair<StatType, StatValueUI> statValueUI in statTypePerSp)
        {
            if (spIncrementValue == 0)
            {
                statValueUI.Value.UpdateNextValue(0);
            }
            else
            {
                statValueUI.Value.UpdateNextValue((currentSpLevel + currentDiceRollValue + spIncrementValue) * statPointSO.KeyValues.FirstOrDefault(x => x.statType == statValueUI.Key).valuePerSp);
            }
        }
    }

    public bool TryUpgradeCurrentValue()
    {
        if (spIncrementValue == 0)
        {
            return false;
        }
        OnLevelUp( currentSpLevel + spIncrementValue);
        return true;    
    }

    public void OnLevelUp(int newLevel)
    {
        spIncrementValue = 0;

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

    public void SetDiceRoll(int diceRoll)
    {
        if (diceRoll == 0)
        {
            attributeDiceRoll.text = "";
            return;
        }
        attributeDiceRoll.text =  "+" + diceRoll.ToString();
    }

    public void OnNewDiceRoll(int diceRoll)
    {
        currentDiceRollValue = diceRoll;
        attributeDiceRoll.text = "+" + diceRoll.ToString();
        int newLevel = currentSpLevel + currentDiceRollValue;
        UpdateCurrentValue();
        UpdateNextStatValues();
    }
}
