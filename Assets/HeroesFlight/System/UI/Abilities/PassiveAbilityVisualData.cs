using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PassiveAbilityVisualData
{
    [SerializeField] protected PassiveAbilityType passiveActiveAbilityType;
    [SerializeField] protected string displayName;
    [SerializeField] protected Sprite icon;
    [TextArea(3, 5)]
    [SerializeField] protected string description;

    public PassiveAbilityType PassiveActiveAbilityType => passiveActiveAbilityType;
    public Sprite Icon => icon;
    public string DisplayName => displayName;
    public string Description => description;
}
