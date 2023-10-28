using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager;
using System;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 0)]
public class CharacterSO : ScriptableObject
{
    [Header("Character Info")]
    [SerializeField] string characterName;
    [SerializeField] CharacterType characterType;
    [SerializeField] bool isUnlockedDefault;
    [SerializeField] bool isSelectedDefault;

    [Header("Others")]
    [SerializeField] AppearanceData appearanceData;
    [SerializeField] PlayerStatData playerStatData;
    [SerializeField] CharacterAnimations m_AnimationData;
    [SerializeField] UltimateData ultimateData;
    [SerializeField] AttackData attackData;
    [Header("References to vfx from asset bank")]
    [SerializeField] VFXData vfxData;
    [Header("UI iNFO")]
    [SerializeField] CharacterUiData characterUiData;

    [Header("Character Data - Modified in run time")]
    [SerializeField] Data characterData;

    public string CharacterName => characterName;
    public CharacterType CharacterType => characterType;
    public PlayerStatData GetPlayerStatData => playerStatData;
    public AppearanceData GetAppearanceData => appearanceData;
    public CharacterAnimations CharacterAnimations => m_AnimationData;
    public UltimateData UltimateData => ultimateData;
    public AttackData AttackData => attackData;
    public VFXData VFXData => vfxData;
    public CharacterUiData CharacterUiData => characterUiData;

    public Data CharacterData => characterData;

    [Serializable]
    public class Data
    {
        public CharacterType characterType;
        public bool isUnlocked;
        public bool isSelected;

        public Data(CharacterSO characterSO)
        {
            characterType = characterSO.characterType;
            isUnlocked = characterSO.isUnlockedDefault;
            isSelected = characterSO.isSelectedDefault;
        }
    }

    public void Unlock()
    {
        characterData.isUnlocked = true;
        Save();
    }

    public void ToggleSelected(bool selected)
    {
        if (selected && characterData.isSelected)
        {
            return;
        }
        characterData.isSelected = selected;
        Save();
    }

    public void Load()
    {
        Data savedCurrencyData = FileManager.Load<Data>(characterType.ToString());
        characterData = savedCurrencyData != null ? savedCurrencyData : new Data(this);
    }

    public void Save()
    {
        FileManager.Save(characterType.ToString(),characterData);
    }
}

[Serializable]
public class CharacterUiData
{
    [Header("Unlocked Info")]
    [SerializeField] private Sprite characterUnlockedImage;
    [SerializeField] private Sprite characterUnlockedName;
    [SerializeField] private Sprite characterClassIcon;
    [SerializeField] private Sprite characterClassName;

    [Header("Locked Info")]
    [SerializeField] private Sprite characterLockedImage;
    [SerializeField] private Sprite characterLockedName;

    public Sprite CharacterUnlockedImage => characterUnlockedImage;
    public Sprite CharacterUnlockedName => characterUnlockedName;
    public Sprite CharacterClassIcon => characterClassIcon;
    public Sprite CharacterClassName => characterClassName;
    public Sprite CharacterLockedImage => characterLockedImage;
    public Sprite CharacterLockedName => characterLockedName;
}