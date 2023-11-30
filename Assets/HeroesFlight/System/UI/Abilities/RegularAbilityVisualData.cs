using UnityEngine;

[System.Serializable]
public class RegularAbilityVisualData
{
    [Header("Regular Ability Visual Data")]
    [SerializeField] protected ActiveType regularActiveType;
    [SerializeField] protected ActiveAbilityType regularActiveAbilityType;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected string displayName;
    [TextArea(3, 5)]
    [SerializeField] protected string description;

    public ActiveAbilityType RegularActiveAbilityType => regularActiveAbilityType;
    public Sprite Icon => icon;
    public string DisplayName => displayName;
    public string Description => description;
}