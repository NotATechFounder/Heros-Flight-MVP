using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AngelCardProperties
{
    public TextMeshProUGUI cardNameDisplay;
    public TextMeshProUGUI cardDescriptionDisplay;
    public Image cardImageDisplay;
    public AngelCardType cardType;
    public AngelCardTier cardTier;
}

public enum AngelCardType
{
    Buff,
    Debuff,
    Blank,
}

public enum AngelCardTier
{
    One,
    Two,
    Three,
}