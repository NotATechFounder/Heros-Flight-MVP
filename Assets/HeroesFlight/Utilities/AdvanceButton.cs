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
    private JuicerRuntime onClickSizeDownEffect;
    private JuicerRuntime onClickSizeUpEffect;

    private JuicerRuntimeCore<Color> onClickDownColor;
    private JuicerRuntimeCore<Color> onClickUpColor;

    //Can be tweat on Debug Mode
    [Space]
    [SerializeField] private bool ChangeChildColor = false;
    private Image[] childImages;

    [Space]
    [SerializeField] private Vector3 ButtonInitialScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 ButtonDownScale = new Vector3(.9f, .9f, .9f);
    [SerializeField] private Vector3 ButtonUpScale = new Vector3(1.1f, 1.1f, 1.1f);

    [Space]
    [SerializeField] private Color buttonDownColor = Color.gray;
    [SerializeField] private Color buttonUpColor = Color.white;

    [Space]
    [SerializeField] private float ButtonDownDuration = 0.1f;
    [SerializeField] private float ButtonUpDuration = 0.25f;
    //

    protected override void Start()
    {
        if (Application.isPlaying)
        {
            if (ChangeChildColor)
            {
                childImages = GetComponentsInChildren<Image>();
            }

            onClickSizeDownEffect = transform.JuicyScale(ButtonDownScale, ButtonDownDuration);
            onClickSizeUpEffect = transform.JuicyScale(ButtonInitialScale, ButtonUpDuration);

            onClickDownColor = transform.GetComponent<Image>().JuicyColour(buttonDownColor, ButtonDownDuration);
            onClickUpColor = transform.GetComponent<Image>().JuicyColour(buttonUpColor, ButtonUpDuration);
        }
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
            //transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            //onClickSizeEffect.Start();

            OnAnyButtonClicked?.Invoke();
            OnToggled();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (interactable)
        {
            onClickDownColor.Start();
            onClickSizeDownEffect.Start();

            if (ChangeChildColor)
            {
                foreach (Image childImage in childImages)
                {
                    if (childImage != onClickDownColor.Target)
                    {
                        JuicerRuntimeCore<Color> childColorAnimation = childImage.JuicyColour(buttonDownColor, ButtonDownDuration);
                        childColorAnimation.Start();
                    }
                }
            }
        }
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (interactable)
        {
            transform.localScale = ButtonUpScale;

            onClickUpColor.Start();
            onClickSizeUpEffect.Start();

            if (ChangeChildColor)
            {
                foreach (Image childImage in childImages)
                {
                    if (childImage != onClickDownColor.Target)
                    {
                        JuicerRuntimeCore<Color> childColorAnimation = childImage.JuicyColour(buttonUpColor, ButtonUpDuration);
                        childColorAnimation.Start();
                    }
                }
            }
        }
    }
}
