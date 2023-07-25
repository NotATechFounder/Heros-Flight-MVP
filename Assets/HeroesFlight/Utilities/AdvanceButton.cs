using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pelumi.Juicer;

public class AdvanceButton : Button
{
    public static event Action OnAnyButtonClicked;

    [SerializeField] private UnityEvent<bool> onClickToggle;

    private bool isOn = false;
    private JuicerRuntime onClickEffect;

    protected override void Start()
    {
        if (Application.isPlaying)
            onClickEffect = transform.JuicyScale(new Vector3(1f, 1f, 1f), .1f);
    }

    public UnityEvent<bool> OnToggle
    {
        get { return onClickToggle; }
        set { onClickToggle = value; }
    }

    public void OnToggled()
    {
        isOn = !isOn;
        onClickToggle?.Invoke(isOn);
    }

    public void SetIsOn(bool value)
    {
        isOn = value;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }
  
     public override void OnPointerClick(PointerEventData eventData)
     {
        base.OnPointerClick(eventData);
        if (interactable)
        {
            transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            onClickEffect.Start();
            OnAnyButtonClicked?.Invoke();
            OnToggled();
        }   
    }
}
