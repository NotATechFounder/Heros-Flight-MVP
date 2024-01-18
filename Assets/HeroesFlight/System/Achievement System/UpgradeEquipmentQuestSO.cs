using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Upgrade Equipment Quest", menuName = "Quest/Upgrade Equipment Quest")]
public class UpgradeEquipmentQuestSO : QuestSO
{
    public Rarity rarity;

    public void Awake()
    {
        questType = QuestType.UpgradeEquipment;
    }
}
