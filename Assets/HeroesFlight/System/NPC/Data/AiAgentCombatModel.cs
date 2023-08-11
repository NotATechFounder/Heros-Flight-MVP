using HeroesFlight.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AiAgentCombatModel", menuName = "Model/Combat/AiAgentCombat", order = 0)]
public class AiAgentCombatModel : ScriptableObject
{
    [SerializeField] private MonsterStatData monsterStatData;

    public MonsterStatData GetMonsterStatData => monsterStatData;
}
