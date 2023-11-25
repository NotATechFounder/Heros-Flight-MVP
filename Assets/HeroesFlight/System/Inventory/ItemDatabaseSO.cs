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
    [SerializeField] CustomAnimationCurve itemMaterialCostStat;

    [Header("Rarities Information")]
    [SerializeField] RarityInfo[] rarityInfos;

    [Header("ItemEffect")]
    [SerializeField] ItemEffectSO[] itemEffects;

    public int GetLevelCost(ItemData currentItem)
    {
        return itemGoldCostStat.GetCurrentValueInt(currentItem.GetValue());
    }
    public int GetCostToLevel(ItemData currentItem)
    {
        ItemSO itemBase = GetItemSOByID(currentItem.ID);
        return itemGoldCostStat.GetCurrentValueInt(currentItem.GetValue()) / 2 + GetRarityInfo((itemBase as EquipmentSO).rarity).defaultSellPrice;
    }

    public int GetItemMaxLevel(Item currentItem)
    {
        return GetRarityInfo((currentItem.itemSO as EquipmentSO).rarity).maxLevel;
    }

    public RarityInfo GetRarityInfo(Rarities currentRarity)
    {
        for (int i = 0; i < rarityInfos.Length; i++) if (currentRarity == rarityInfos[i].rarity) return rarityInfos[i];
        return null;
    }

    public void SetItemBuffStat(Item currentItem)
    {
        ItemSO itemBase = currentItem.itemSO;

        for (int i = 0; i < currentItem.ItemBuffs().Length; i++)
        {
            for (int j = 0; j < itemEffects.Length; j++)
            {
                if (currentItem.ItemBuffs()[i].itemEffectType != itemEffects[j].buffType) continue;

                for (int k = 0; k < itemEffects[j].buffRarityStats.Length; k++)
                {
                    if (itemEffects[j].buffRarityStats[k].rarity == (itemBase as EquipmentSO).rarity)
                    {
                        currentItem.ItemBuffs()[i].value = itemEffects[j].buffRarityStats[k].statCurve.GetCurrentValueInt(currentItem.ItemData().GetValue());
                        break;
                    }
                }
            }
        }
    }

    public int GetBuffValue(ItemEffectType buff, Rarities currentRarity, int level)
    {
        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (buff != itemEffects[i].buffType) continue;
            for (int j = 0; j < itemEffects[i].buffRarityStats.Length; j++)
            {
                if (itemEffects[i].buffRarityStats[j].rarity == currentRarity) return itemEffects[i].buffRarityStats[j].statCurve.GetCurrentValueInt(level);
            }
        }
        return 0;
    }

    public BuffInfo GetBuffInfo(ItemEffectType buff)
    {
        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (buff == itemEffects[i].buffType) return itemEffects[i].buffInfo;
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

    [ContextMenu("PopulateItemEffects")]
    public void PopulateItemEffects()
    {
        string path = AssetDatabase.GetAssetPath(this);
        path = path.Replace(this.name + ".asset", "");
        List<ItemEffectSO> scriptableObjectBases = ScriptableObjectUtils.GetAllScriptableObjectBaseInFile<ItemEffectSO>(path);
        itemEffects = scriptableObjectBases.ToArray();
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

#endif
}
