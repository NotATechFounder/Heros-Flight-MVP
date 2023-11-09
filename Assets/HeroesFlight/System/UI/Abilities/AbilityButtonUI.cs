using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AbilityButtonUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _infoText;
    public event Action<PassiveActiveAbilityType> OnAbilitySelected;

    private AdvanceButton _advanceButton;

    public AdvanceButton GetAdvanceButton => _advanceButton;

    public void SetInfo(AbilityVisualData abilityVisualData, Action<PassiveActiveAbilityType> OnAbilitySelected)
    {
        _icon.sprite = abilityVisualData.Icon;
        _nameText.text = abilityVisualData.PassiveActiveAbilityType.ToString();
        _infoText.text = abilityVisualData.Description;
        this.OnAbilitySelected = OnAbilitySelected;
        _advanceButton.onClick.AddListener(() => this.OnAbilitySelected.Invoke(abilityVisualData.PassiveActiveAbilityType));
    }

    private void Awake()
    {
        _advanceButton = GetComponent<AdvanceButton>();
    }
}
