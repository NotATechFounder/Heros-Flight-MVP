using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class AngelCardProperties
{
    public TextMeshProUGUI cardNameDisplay;
    public TextMeshProUGUI cardDescriptionDisplay;
    public Image cardImageDisplay;
}

public class TierEffect
{
    public AngelCardTier tier;
    public float effect;
}

public enum AngelCardType
{
    Buff,
    Debuff,
}

public enum AngelCardTier
{
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
}