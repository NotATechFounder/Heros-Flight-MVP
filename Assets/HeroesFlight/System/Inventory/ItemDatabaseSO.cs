using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory System/Item Database")]
public class ItemDatabaseSO : ScriptableObjectDatabase<ItemSO>
{
    [Header("Item Cost Stats")]
    [SerializeField] CustomAnimationCurve itemGoldCostStat;

    [Header("Rarities")]
    [SerializeField] RarityInfo[] rarityInfos;
    [Header("Buffs")]
    [SerializeField] ItemEffectSO[] buffs;

    public int GetLevelCost(ItemData currentItem)
    {
        return itemGoldCostStat.GetCurrentValueInt(currentItem.GetValue());
    }
    public int GetCostToLevel(ItemData currentItem)
    {
        ItemSO itemBase = GetItemSOByID(currentItem.ID);
        return itemGoldCostStat.GetCurrentValueInt(currentItem.GetValue()) / 2 + GetRarityInfo((itemBase as EquipmentObject).rarity).defaultSellPrice;
    }

    public int GetItemMaxLevel(Item currentItem)
    {
        return GetRarityInfo((currentItem.itemObject as EquipmentObject).rarity).maxLevel;
    }

    public RarityInfo GetRarityInfo(Rarities currentRarity)
    {
        for (int i = 0; i < rarityInfos.Length; i++) if (currentRarity == rarityInfos[i].rarity) return rarityInfos[i];
        return null;
    }

    public void SetItemBuffStat(Item currentItem)
    {
        ItemSO itemBase = currentItem.itemObject;

        for (int i = 0; i < currentItem.ItemBuffs().Length; i++)
        {
            for (int j = 0; j < buffs.Length; j++)
            {
                if (currentItem.ItemBuffs()[i].itemEffectType != buffs[j].buffType) continue;

                for (int k = 0; k < buffs[j].buffRarityStats.Length; k++)
                {
                    if (buffs[j].buffRarityStats[k].rarity == (itemBase as EquipmentObject).rarity)
                    {
                        currentItem.ItemBuffs()[i].value = buffs[j].buffRarityStats[k].statCurve.GetCurrentValueInt(currentItem.ItemData().GetValue());
                        break;
                    }
                }
            }
        }
    }

    public int GetBuffValue(ItemEffectType buff, Rarities currentRarity, int level)
    {
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buff != buffs[i].buffType) continue;
            for (int j = 0; j < buffs[i].buffRarityStats.Length; j++)
            {
                if (buffs[i].buffRarityStats[j].rarity == currentRarity) return buffs[i].buffRarityStats[j].statCurve.GetCurrentValueInt(level);
            }
        }
        return 0;
    }

    public BuffInfo GetBuffInfo(ItemEffectType buff)
    {
        for (int i = 0; i < buffs.Length; i++)
        {
            if (buff == buffs[i].buffType) return buffs[i].buffInfo;
        }
        return default;
    }

#if UNITY_EDITOR
    [ContextMenu("RenameAllItems")]
    public void RenameAllItems()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RenameFile();
        }
    }
#endif
}
