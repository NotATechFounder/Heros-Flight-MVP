using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityVisualData
{
    [SerializeField] protected PassiveActiveAbilityType passiveActiveAbilityType;
    [Header("UI")]
    [SerializeField] protected Sprite icon;
    [TextArea(3, 5)]
    [SerializeField] protected string description;

    public PassiveActiveAbilityType PassiveActiveAbilityType => passiveActiveAbilityType;
    public Sprite Icon => icon;
    public string Description => description;
}
