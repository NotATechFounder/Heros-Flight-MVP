using HeroesFlight.Common.Enum;
using UnityEngine;

[CreateAssetMenu(fileName = "New Obtain Equipment Quest", menuName = "Quest/Obtain Equipment Quest")]
public class ObtainEquipmentQuestSO : QuestSO
{
    public Rarity rarity;

    public override void Awake()
    {
        questType = QuestType.ObtainEquipment;
    }
}
