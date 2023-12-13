
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Defeat Mob Quest", menuName = "Quest/Defeat Mob Quest")]
public class DefeatMobQuestSO : QuestSO
{
    public WorldType worldType;

    public override void Awake()
    {
        questType = QuestType.DefeatMob;
    }
}