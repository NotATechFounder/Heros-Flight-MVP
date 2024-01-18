
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defeat World Boss Quest", menuName = "Quest/Defeat World Boss Quest")]

public class DefeatWorldBossSO : QuestSO
{
    public WorldType worldType;

    public void Awake()
    {
        questType = QuestType.DefeatWorldBoss;
    }
}
