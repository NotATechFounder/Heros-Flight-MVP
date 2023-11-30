using UnityEngine;

[System.Serializable]
public class ActiveAbilityVisualData
{
    [Header("Regular Ability Visual Data")]
    [SerializeField] protected ActiveType regularActiveType;
    [SerializeField] protected ActiveAbilityType regularActiveAbilityType;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected string displayName;
    [TextArea(3, 5)]
    [SerializeField] protected string description;

    public ActiveType ActiveType => regularActiveType;
    public ActiveAbilityType ActiveAbilityType => regularActiveAbilityType;
    public Sprite Icon => icon;
    public string DisplayName => displayName;
    public string Description => description;
}