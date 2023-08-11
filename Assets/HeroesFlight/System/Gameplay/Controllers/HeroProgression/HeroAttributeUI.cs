using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class HeroAttributeUI : MonoBehaviour
{
    public Func<bool> OnUpButtonClickedEvent;
    public Func<bool> OnDownButtonClickedEvent;
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
    }

    public void OnSPChanged(int sp)
    {
        attributeValue.text = sp.ToString();
    }

    public void OnUpButtonClicked()
    {
        if(OnUpButtonClickedEvent.Invoke())
        {
            attributeInfo.IncrementSP();
            attributeValue.text = attributeInfo.CurrentSP.ToString();
        }
    }

    public void OnDownButtonClicked()
    {
        if (OnUpButtonClickedEvent.Invoke())
        {
            attributeInfo.DecrementSP();
            attributeValue.text = attributeInfo.CurrentSP.ToString();
        }
    }

    public void OnInfoButtonClicked()
    {
        OnInfoButtonClickedEvent?.Invoke(attributeInfo.AttributeSO);
    }
}
