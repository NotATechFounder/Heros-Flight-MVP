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

    private AdvanceButton _advanceButton;

    public AdvanceButton GetAdvanceButton => _advanceButton;

    public void SetInfo(Sprite icon, string name, string info)
    {
        _icon.sprite = icon;
        _nameText.text = name;
        _infoText.text = info;

    }
    private void Awake()
    {
        _advanceButton = GetComponent<AdvanceButton>();
    }
}
