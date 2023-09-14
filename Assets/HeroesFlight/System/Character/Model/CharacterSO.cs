using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 0)]
public class CharacterSO : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] CharacterType characterType;
    [SerializeField] AppearanceData appearanceData;
    [SerializeField] PlayerStatData playerStatData;
    [SerializeField] AnimationData aniamtionData;
    [SerializeField] UltimateData ultimateData;
    [SerializeField] AttackData attackData;
    [Header("References to vfx from asset bank")]
    [SerializeField] VFXData vfxData;

    public string CharacterName => characterName;
    public PlayerStatData GetPlayerStatData => playerStatData;
    public AppearanceData GetAppearanceData => appearanceData;
    public AnimationData AnimationData => aniamtionData;
    public UltimateData UltimateData => ultimateData;
    public AttackData AttackData => attackData;
    public CharacterType CharacterType => characterType;
    public VFXData VFXData => vfxData;
}