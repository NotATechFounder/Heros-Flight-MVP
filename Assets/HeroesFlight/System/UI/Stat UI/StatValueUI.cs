using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatValueUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statName;
    [SerializeField] private TextMeshProUGUI statCurrentValueText;
    [SerializeField] private TextMeshProUGUI statNextValueText;

    private StatType statType;

    public StatType StatType => statType;

    public void Init(StatType statType, string statCurrentValue)
    {
        this.statType = statType;
        statName.text = statType.ToString();
        statCurrentValueText.text = statCurrentValue;
        statNextValueText.gameObject.SetActive(false);
    }

    public void UpdateCurrentValue(string statCurrentValue)
    {
        statCurrentValueText.text = statCurrentValue;
    }

    public void UpdateNextValue(float statNextValue)
    {
        statNextValueText.text = statNextValue.ToString();
        statNextValueText.gameObject.SetActive(statNextValue != 0);   
    }

    public void ConfrimNewValues()
    {
        statCurrentValueText.text = statNextValueText.text;
        statNextValueText.gameObject.SetActive(false);
    }
}
