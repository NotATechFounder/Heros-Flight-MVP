using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Pelumi.Juicer;

public enum GameButtonVisiblity
{
    Visible,
    Hidden
}

public class AdvanceButton : Button
{
    public static event Action OnAnyButtonClicked;

    [SerializeField] private GameButtonType buttonType;
    [SerializeField] private UnityEvent<bool> onClickToggle;

    private bool isOn = false;
    private JuicerRuntime onClickDownSizeEffect;
    private JuicerRuntime onClickUpSizeEffect;

    //private JuicerRuntimeCore<Color> onClickDownColorEffect;
    //private JuicerRuntimeCore<Color> onClickUpColorEffect;

    private Image buttonImage;
    private Image[] childImages;

    [Space]
    [SerializeField] private bool ChangeChildColor = false;

    [Space]
    [SerializeField] private Vector3 ButtonInitialScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 ButtonDownScale = new Vector3(.9f, .9f, .9f);
    [SerializeField] private Vector3 ButtonUpScale = new Vector3(1.1f, 1.1f, 1.1f);

    [Space]
    [SerializeField] public Color buttonDownColor = Color.gray;
    [SerializeField] public Color buttonUpColor = Color.white;

    [Space]
    [SerializeField] private float ButtonDownDuration = 0.1f;
    [SerializeField] private float ButtonUpDuration = 0.25f;

    private GameButtonVisiblity gameButtonVisiblity;

    public GameButtonType ButtonType { get { return buttonType; } }
    public GameButtonVisiblity GetGameButtonVisiblity { get { return gameButtonVisiblity; } }

    protected override void Awake()
    {
        base.Awake();
        buttonImage = transform.GetComponent<Image>();
        childImages = GetComponentsInChildren<Image>();
    }

    protected override void Start()
    {
        if (Application.isPlaying)
        {
            onClickDownSizeEffect = transform.JuicyScale(ButtonDownScale, ButtonDownDuration);
            onClickUpSizeEffect = transform.JuicyScale(ButtonInitialScale, ButtonUpDuration);

            if(buttonImage)
            {
                //onClickDownColorEffect = buttonImage.JuicyColour(buttonDownColor, ButtonDownDuration);
                //onClickUpColorEffect = buttonImage.JuicyColour(buttonUpColor, ButtonUpDuration);
            }
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
        base.OnPointerDown(eventData);
        if (interactable)
        {
            onClickDownSizeEffect.Start();

            //onClickDownColorEffect.Start();

            //if (ChangeChildColor)
            //{
            //    foreach (Image childImage in childImages)
            //    {
            //        if (childImage != buttonImage)
            //        {
            //            JuicerRuntimeCore<Color> childColorAnimation = childImage.JuicyColour(buttonDownColor, ButtonDownDuration);
            //            childColorAnimation.Start();
            //        }
            //    }
            //}
        }
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (interactable)
        {
            onClickUpSizeEffect.Start(()=> transform.localScale = ButtonUpScale);

            //onClickUpColorEffect.Start();

            //if (ChangeChildColor)
            //{
            //    foreach (Image childImage in childImages)
            //    {
            //        if (childImage != buttonImage)
            //        {
            //            JuicerRuntimeCore<Color> childColorAnimation = childImage.JuicyColour(buttonUpColor, ButtonUpDuration);
            //            childColorAnimation.Start();
            //        }
            //    }
            //}
        }
    }

    public void SetVisibility(GameButtonVisiblity state)
    {
        gameButtonVisiblity = state;
        interactable = state == GameButtonVisiblity.Visible;

        foreach (Image childImage in childImages)
        {
            if (childImage == buttonImage) continue;
            childImage.color = state == GameButtonVisiblity.Visible ? Color.white : Color.grey;
        }
    }
}
