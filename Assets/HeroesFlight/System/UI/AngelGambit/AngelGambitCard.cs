using Pelumi.Juicer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelGambitCard : MonoBehaviour
{
    public Action<AngelCardType> OnClicked;

    [SerializeField] private AngelCardType angelCardType;
    private AdvanceButton cardButton;
    private JuicerRuntime cardEffect;

    private void Awake()
    {
        cardButton = GetComponent<AdvanceButton>();
        cardButton.onClick.AddListener(Clicked);
        cardEffect = transform.JuicyScale(Vector3.one * 1.05f, .7f).SetEase(Ease.EaseOutSine).SetLoop(-1);
    }

    public void Initialize(Action<AngelCardType> clickedEvent)
    {
        OnClicked = clickedEvent;
    }

    public void Clicked()
    {
        OnClicked?.Invoke(angelCardType);
    }

    public void ResetCard()
    {
       transform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        cardEffect.Start();
    }

    void OnDisable()
    {
        cardEffect.Pause();
    }
}
