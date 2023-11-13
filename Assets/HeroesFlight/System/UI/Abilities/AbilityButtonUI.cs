using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using log4net.Core;

public class AbilityButtonUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _infoText;
    [SerializeField] private TextMeshProUGUI levelText;

    private AdvanceButton _advanceButton;

    public AdvanceButton GetAdvanceButton => _advanceButton;

    private void Awake()
    {
        _advanceButton = GetComponent<AdvanceButton>();
    }

    public void SetInfo(Sprite icon,string typeText,  string name, string info , int currentLevel = 1)
    {
        _icon.sprite = icon;
        this.typeText.text = typeText;
        _nameText.text = name;
        _infoText.text = info;
        DisplayLevel (currentLevel);
    }

    public void DisplayLevel(int currentLvl)
    {
        if (currentLvl != 0)
        {

            levelText.text = $"Lvl. {currentLvl}" + " >> " + (currentLvl + 1);
        }
        else
        {
            levelText.text = $"Lvl. {currentLvl + 1}";
        }
    }
}
