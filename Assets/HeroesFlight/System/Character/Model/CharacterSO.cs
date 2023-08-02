using HeroesFlight.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character", order = 0)]
public class CharacterSO : ScriptableObject
{
    [SerializeField] string characterName;
    [SerializeField] AppearanceData appearanceData;

    [SerializeField] PlayerStatData playerStatData;

    public string CharacterName => characterName;
    public PlayerStatData GetPlayerStatData => playerStatData;
    public AppearanceData GetAppearanceData => appearanceData;
}
