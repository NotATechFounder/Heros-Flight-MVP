using ScriptableObjectDatabase;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory System/Item Database")]
public class ItemDatabaseSO : ScriptableObjectDatabase<ItemSO>
{
    [Header("Item Cost Stats")]
    [SerializeField] CustomAnimationCurve itemUpgradeGoldCost;
    [SerializeField] CustomAnimationCurve itemUpgradeMaterialCost;

    [Header("Rarities Information")]
    [SerializeField] RarityInfo[] rarityInfos;

    [Header("ItemEffect")]
    [SerializeField] ItemEffectSO[] itemEffects;

    private void OnValidate()
    {
        itemUpgradeGoldCost.UpdateCurve();
        itemUpgradeMaterialCost.UpdateCurve();
    }

    public int GetUpgradeGoldCost(ItemData currentItem)
    {
        return itemUpgradeGoldCost.GetCurrentValueInt(currentItem.GetValue());
    }

    public int GetTotalUpgradeGoldCost(ItemData currentItem)
    {
        ItemSO itemSO = GetItemSOByID(currentItem.ID);
        return itemUpgradeGoldCost.GetTotalValue(currentItem.GetValue()) + GetRarityInfo((itemSO as EquipmentSO).rarity).defaultDisamatlePrice;
    }

    public int GetUpgradeMaterialCost(ItemData currentItem)
    {
        return itemUpgradeMaterialCost.GetCurrentValueInt(currentItem.GetValue());
    }

    public int GetTotalUpgradeMaterialCost(ItemData currentItem)
    {
        ItemSO itemSO = GetItemSOByID(currentItem.ID);
        return itemUpgradeMaterialCost.GetTotalValue(currentItem.GetValue()) + GetRarityInfo((itemSO as EquipmentSO).rarity).defaultMaterial;
    }

    public int GetItemMaxLevel(Item currentItem)
    {
        return GetRarityInfo(currentItem.GetItemSO<EquipmentSO>().rarity).maxLevel;
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

    public void SetItemBuffStat(Item currentItem)
    {
        ItemSO itemBase = currentItem.itemSO;

        for (int i = 0; i < currentItem.ItemBuffs().Length; i++)
        {
            for (int j = 0; j < itemEffects.Length; j++)
            {
                if (currentItem.ItemBuffs()[i].itemEffectType != itemEffects[j].itemEffectType) continue;

                for (int k = 0; k < itemEffects[j].effectRarityStats.Length; k++)
                {
                    if (itemEffects[j].effectRarityStats[k].rarity == (itemBase as EquipmentSO).rarity)
                    {
                        currentItem.ItemBuffs()[i].value = itemEffects[j].effectRarityStats[k].statCurve.GetCurrentValueInt(currentItem.GetItemData().GetValue());
                        break;
                    }
                }
            }
        }
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

    public ItemSO GetNextRarityItemSO(ItemSO itemSO)
    {
        Rarity currentRarity = (itemSO as EquipmentSO).rarity;
        for (int i = 0; i < Items.Length; i++)
        {
            if(itemSO.Name == Items[i].Name && (Items[i] as EquipmentSO).rarity == currentRarity + 1)
            {
                return Items[i];
            }
        }
        return null;
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
