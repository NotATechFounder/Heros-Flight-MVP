using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using UnityEditor;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory System/Item Database")]
public class ItemDatabaseSO : ScriptableObjectDatabase<ItemSO>
{

    [Serializable]
    public class StarStat
    {
        public EquipmentStar equipmentStar;
        public RarityValue[] rarityValues;
    }

    [Serializable]
    public class RarityValue
    {
        public Rarity rarity;
        public int value;
    }

    [Serializable]
    public class EquipmentStat
    {
        public EquipmentType equipmentType;
        public StarStat[] starStats;
    }

    [Header("Item Cost Stats")]
    [SerializeField] CustomAnimationCurve itemUpgradeGoldCost;
    [SerializeField] CustomAnimationCurve itemUpgradeMaterialCost;

    [Header("Rarities Information")]
    [SerializeField] RarityInfo[] rarityInfos;

    [Header("Base Stats")]
    [SerializeField] EquipmentStat[] baseStats;
    [SerializeField] EquipmentStatIncrease[] baseStatIncrease;

    [Header("ItemEffect")]
    [SerializeField] ItemEffectSO[] itemEffects;

    public EquipmentStat[] equipmentBaseStat => baseStats;

    private void OnValidate()
    {
        itemUpgradeGoldCost.UpdateCurve();
        itemUpgradeMaterialCost.UpdateCurve();
    }

    public int GetUpgradeGoldCost(ItemData currentItem)
    {
        return itemUpgradeGoldCost.GetCurrentValueInt(currentItem.GetValue());
    }

    public int GetEquipmentTotalUpgradeGoldCost(ItemEquipmentData currentItem)
    {
        return itemUpgradeGoldCost.GetTotalValue(currentItem.GetValue()) + GetRarityInfo(currentItem.rarity).defaultDisamatlePrice;
    }

    public int GetUpgradeMaterialCost(ItemData currentItem)
    {
        return itemUpgradeMaterialCost.GetCurrentValueInt(currentItem.GetValue());
    }

    public int GetEquipmentTotalUpgradeMaterialCost(ItemEquipmentData currentItem)
    {
        return itemUpgradeMaterialCost.GetTotalValue(currentItem.GetValue()) + GetRarityInfo(currentItem.rarity).defaultMaterial;
    }

    public int GetItemMaxLevel(Item currentItem)
    {
        return GetItemMaxLevel (currentItem.GetItemData<ItemEquipmentData>().rarity);
    }

    public int GetItemMaxLevel(Rarity rarity)
    {
        return GetRarityInfo(rarity).maxLevel;
    }

    public int GetNextRarityFuseRequirement(Item currentItem)
    {
        return GetRarityInfo(currentItem.GetItemData<ItemEquipmentData>().rarity).nextRarityFuseRequirement;
    }

    public RarityInfo GetRarityInfo(Rarity currentRarity)
    {
        for (int i = 0; i < rarityInfos.Length; i++) if (currentRarity == rarityInfos[i].rarity) return rarityInfos[i];
        return null;
    }

    public RarityPalette GetRarityPalette(Rarity rarity)
    {
        for (int i = 0; i < rarityInfos.Length; i++)
        {
            if (rarityInfos[i].rarity == rarity)
                return rarityInfos[i].palette;
        }
        return null;
    }

    public ItemSO GetRandomItem()
    {
        return Items[Random.Range(0, Items.Length)];
    }

    public ItemSO GetRandomItem(ItemType itemType)
    {
        do
        {
            ItemSO item = Items[Random.Range(0, Items.Length)];
            if (item.itemType == itemType) return item;
        } while (true);
    }

    public int GetBuffValue(ItemEffectType buff, Rarity currentRarity, int level)
    {
        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (buff != itemEffects[i].itemEffectType) continue;
            for (int j = 0; j < itemEffects[i].effectRarityStats.Length; j++)
            {
                if (itemEffects[i].effectRarityStats[j].rarity == currentRarity) return itemEffects[i].effectRarityStats[j].statCurve.GetCurrentValueInt(level);
            }
        }
        return 0;
    }

    public EffectInfo GetBuffInfo(ItemEffectType buff)
    {
        for (int i = 0; i < itemEffects.Length; i++)
        {
            if (buff == itemEffects[i].itemEffectType) return itemEffects[i].buffInfo;
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

    public int GetBaseStatValue(EquipmentSO equipmentSO, Rarity rarity)
    {
        for (int i = 0; i < baseStats.Length; i++)
        {
            if (baseStats[i].equipmentType == equipmentSO.equipmentType)
            {
                for (int j = 0; j < baseStats[i].starStats.Length; j++)
                {
                    if (baseStats[i].starStats[j].equipmentStar == equipmentSO.equipmentStar)
                    {
                        for (int k = 0; k < baseStats[i].starStats[j].rarityValues.Length; k++)
                        {
                            if (baseStats[i].starStats[j].rarityValues[k].rarity == rarity)
                            {
                                return baseStats[i].starStats[j].rarityValues[k].value;
                            }
                        }
                    }
                }
            }
        }
        return 0;
    }

    public int GetBaseStatIncrease(EquipmentSO equipmentSO)
    {
        for (int i = 0; i < baseStatIncrease.Length; i++)
        {
            if (baseStatIncrease[i].equipmentType == equipmentSO.equipmentType)
            {
                for (int j = 0; j < baseStatIncrease[i].starIncreases.Length; j++)
                {
                    if (baseStatIncrease[i].starIncreases[j].equipmentStar == equipmentSO.equipmentStar)
                    {
                        return baseStatIncrease[i].starIncreases[j].incrementPerLevel;
                    }
                }
            }
        }
        return 0;
    }

    public int GetCurrentStatValue(EquipmentSO equipmentSO, Rarity rarity, int level)
    {
        int baseStatValue = GetBaseStatValue(equipmentSO, rarity);
        int incrementPerLevel = GetBaseStatIncrease(equipmentSO);
        return baseStatValue + incrementPerLevel * (level - 1);
    }

#endif
}
