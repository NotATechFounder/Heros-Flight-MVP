using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityRerollButtonUI : MonoBehaviour
{
    public Action OnClick;
    public Action<bool> OnToggle;

    [SerializeField] private Image icon;
    [SerializeField] private Image toggleIcon;
    [SerializeField] private bool toggable;
    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(Toggled);
        Disable();
    }

    public void Init(Sprite icon)
    {
        this.icon.sprite = icon;
        gameObject.SetActive(true); 
    }

    public void Toggled (bool isOn)
    {
        if (toggable)
        {
            toggleIcon.gameObject.SetActive(isOn);
            OnToggle?.Invoke(isOn);
        }
        else
        {
            OnClick?.Invoke();
        }

    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
