using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pelumi.Juicer;
using System.Collections.Generic;

public enum GameButtonVisiblity
{
    Visible,
    Hidden
}

public class AdvanceButton : Button
{
    public static event Action OnAnyButtonClicked;
    public static Action<GameButtonType> OnAnyButtonPointerDown;

    [SerializeField] private GameButtonType buttonType;
    [SerializeField] private UnityEvent<bool> onClickToggle;

    private bool isOn = false;
    private JuicerRuntime onClickDownSizeEffect;
    private JuicerRuntime onClickUpSizeEffect;

    private Image[] childImages = new Image[0];
    private Dictionary<Image, Color> childDefaultImageColors = new Dictionary<Image, Color>();

    [Space]
    [SerializeField] private Vector3 buttonInitialScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 buttonDownScale = new Vector3(.9f, .9f, .9f);
    [SerializeField] private Vector3 buttonUpScale = new Vector3(1.1f, 1.1f, 1.1f);

    [Space]
    [SerializeField] public Color buttonDownColor = Color.gray;
    [SerializeField] public Color buttonUpColor = Color.white;

    [Space]
    [SerializeField] private float buttonDownDuration = 0.1f;
    [SerializeField] private float buttonUpDuration = 0.25f;

    private GameButtonVisiblity gameButtonVisiblity;

    public GameButtonType ButtonType { get { return buttonType; } }
    public GameButtonVisiblity GetGameButtonVisiblity { get { return gameButtonVisiblity; } }

    protected override void Awake()
    {
        base.Awake();
        childImages = GetComponentsInChildren<Image>();
        foreach (Image childImage in childImages)
        {
            childDefaultImageColors.Add(childImage, childImage.color);
        }
    }

    protected override void Start()
    {
        if (Application.isPlaying)
        {
            onClickDownSizeEffect = transform.JuicyScale(buttonDownScale, buttonDownDuration).SetTimeMode(TimeMode.Unscaled);
            onClickUpSizeEffect = transform.JuicyScale(buttonInitialScale, buttonUpDuration).SetTimeMode(TimeMode.Unscaled);
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
        if (gameButtonVisiblity == GameButtonVisiblity.Hidden)
        {
            return;
        }

        base.OnPointerClick(eventData);
        if (interactable)
        {
            OnAnyButtonClicked?.Invoke();
            OnToggled();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        OnAnyButtonPointerDown?.Invoke(buttonType);

        if (gameButtonVisiblity == GameButtonVisiblity.Hidden)
        {
            return;
        }

        base.OnPointerDown(eventData);
        if (interactable)
        {
            onClickDownSizeEffect.Start();
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        if (gameButtonVisiblity == GameButtonVisiblity.Hidden)
        {
            return;
        }

        base.OnPointerDown(eventData);
        if (interactable)
        {
            onClickUpSizeEffect.Start(()=> transform.localScale = buttonUpScale);
        }
    }

    public void SetVisibility(GameButtonVisiblity state)
    {
        gameButtonVisiblity = state;
        interactable = state == GameButtonVisiblity.Visible;

        foreach (Image childImage in childImages)
        {
            childImage.color = state == GameButtonVisiblity.Visible ? childDefaultImageColors[childImage] : Color.grey;
        }
    }
}
