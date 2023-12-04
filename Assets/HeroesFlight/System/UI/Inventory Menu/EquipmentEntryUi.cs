using System;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using UnityEngine;
using UnityEngine.Serialization;

namespace HeroesFlight.System.UI.Inventory_Menu
{
    [Serializable]
    public class EquipmentEntryUi : InventoryItemUiEntry
    {
        public EquipmentEntryUi(string id, Sprite icon, int value, ItemType itemType, RarityPalette rarityPallete,
            string name, string description, EquipmentType equipmentType, bool isEquipped, Rarity itemRarity, string instanceId) : base(id,
            icon, value, itemType, rarityPallete, name, description)
        {
            EquipmentType = equipmentType;
            IsEquipped = isEquipped;
            ItemRarity = itemRarity;
            InstanceId = instanceId;
        }

        public string InstanceId;
        public EquipmentType EquipmentType;
        public bool IsEquipped;
        public Rarity ItemRarity;
    }
}