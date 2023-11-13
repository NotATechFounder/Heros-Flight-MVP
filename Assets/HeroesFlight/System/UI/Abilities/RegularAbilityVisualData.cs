using UnityEngine;

[System.Serializable]
public class RegularAbilityVisualData
{
    [SerializeField] protected RegularActiveAbilityType regularActiveAbilityType;
    [SerializeField] protected Sprite icon;
    [SerializeField] protected string displayName;
    [TextArea(3, 5)]
    [SerializeField] protected string description;

    private int level = 0;

    public RegularActiveAbilityType RegularActiveAbilityType => regularActiveAbilityType;
    public Sprite Icon => icon;
    public string DisplayName => displayName;
    public string Description => description;

    public int Level => level;
}