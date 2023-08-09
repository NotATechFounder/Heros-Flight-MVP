using HeroesFlight.Common;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 0)]
public class CharacterSO : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] AppearanceData appearanceData;
    [SerializeField] PlayerStatData playerStatData;
    [SerializeField] AnimationData aniamtionData;
    [SerializeField] UltimateData ultimateData;
    [SerializeField] AttackData attackData;

    public string CharacterName => characterName;
    public PlayerStatData GetPlayerStatData => playerStatData;
    public AppearanceData GetAppearanceData => appearanceData;
    public AnimationData AnimationData => aniamtionData;
    public UltimateData UltimateData => ultimateData;
    public AttackData AttackData => attackData;
}
