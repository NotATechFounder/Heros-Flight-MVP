using HeroesFlight.Common;
using HeroesFlight.System.Character.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 0)]
public class CharacterSO : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] CharacterType characterType;
    [SerializeField] HeroType heroType;
    [SerializeField] AppearanceData appearanceData;
    [SerializeField] PlayerStatData playerStatData;
    [SerializeField] AnimationData aniamtionData;
    [SerializeField] UltimateData ultimateData;
    [SerializeField] AttackData attackData;

    public string CharacterName => characterName;
    public HeroType HeroType => heroType;
    public PlayerStatData GetPlayerStatData => playerStatData;
    public AppearanceData GetAppearanceData => appearanceData;
    public AnimationData AnimationData => aniamtionData;
    public UltimateData UltimateData => ultimateData;
    public AttackData AttackData => attackData;
    public CharacterType CharacterType => characterType;
}


public enum HeroType
{
    Melee,
    Ranged
}