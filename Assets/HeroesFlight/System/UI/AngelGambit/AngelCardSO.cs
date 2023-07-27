using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AngelCard", menuName = "Angel Gambit/Angel Card")]
public class AngelCardSO : ScriptableObject
{
    public string cardName;
    public string cardDescription;
    public Sprite cardImage;
    public AngelCardType cardType;
    public AngelCardTier cardTier;
}
