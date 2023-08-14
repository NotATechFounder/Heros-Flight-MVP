using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class HeroAttributeUI : MonoBehaviour
{
    public Action<HeroProgressionAttributeInfo> OnUpButtonClickedEvent;
    public Action<HeroProgressionAttributeInfo> OnDownButtonClickedEvent;
    public Action<HPAttributeSO> OnInfoButtonClickedEvent;

    [SerializeField] private AdvanceButton upButton;
    [SerializeField] private AdvanceButton downButton;
    [SerializeField] private AdvanceButton infoButton;
    [SerializeField] private TextMeshProUGUI attributeName;
    [SerializeField] private TextMeshProUGUI attributeValue;
    [SerializeField] private Image icon; 

    private HeroProgressionAttributeInfo attributeInfo;

    private void Awake()
    {
        upButton.onClick.AddListener(OnUpButtonClicked);
        downButton.onClick.AddListener(OnDownButtonClicked);
        infoButton.onClick.AddListener(OnInfoButtonClicked);
    }

    public void SetAttribute(HeroProgressionAttributeInfo attribute)
    {
        this.attributeInfo = attribute;
        attributeName.text = attribute.AttributeSO.Attribute.ToString();
        attributeValue.text = attribute.CurrentSP.ToString();
        icon.sprite = attribute.AttributeSO.Icon;
        attribute.OnSPChanged = OnSPChanged;
        attribute.OnModified = OnModified;
    }

    public void OnSPChanged(int sp)
    {
        attributeValue.text = sp.ToString();
    }

    public void OnModified(bool modified)
    {
        attributeValue.color = modified ? Color.green : Color.white;
    }

    public void OnUpButtonClicked()
    {
        OnUpButtonClickedEvent?.Invoke(attributeInfo);
    }

    public void OnDownButtonClicked()
    {
        OnDownButtonClickedEvent?.Invoke(attributeInfo);
    }

    public void OnInfoButtonClicked()
    {
        OnInfoButtonClickedEvent?.Invoke(attributeInfo.AttributeSO);
    }

    public void ResetSpText()
    {
        attributeValue.text = 0.ToString();
    }
}
