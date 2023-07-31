using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngelCardUI : MonoBehaviour
{
    public event Action<AngelCardSO> OnSelected;

    [SerializeField] private AngelCardProperties angelCardProperties;
    [SerializeField] private AdvanceButton advanceButton;

    private AngelCardType cardType;
    private AngelCardTier cardTier;
    private AngelCardSO angelCardSO;

    private void Start()
    {
        advanceButton.onClick.AddListener(SelectCard);
    }

    public void Init(AngelCardSO angelCard)
    {
        angelCardSO = angelCard;
        angelCardProperties.cardNameDisplay.text = angelCard.CardName;
        angelCardProperties.cardDescriptionDisplay.text = angelCard.ShortCardDescription;
        angelCardProperties.cardImageDisplay.sprite = angelCard.CardImage;
        cardType = angelCard.CardType;
        cardTier = angelCard.CardTier;
    }

    public void SelectCard()
    {
        OnSelected?.Invoke(angelCardSO);
    }
}
