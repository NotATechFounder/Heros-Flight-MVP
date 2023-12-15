using System;
using System.Collections.Generic;
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
            string name, string description, EquipmentType equipmentType, bool isEquipped, Rarity itemRarity, List<ItemEffectEntryUi> itemEffectEntryUis) : base(id,
            icon, value, itemType, rarityPallete, name, description)
        {
            EquipmentType = equipmentType;
            IsEquipped = isEquipped;
            ItemRarity = itemRarity;
            this.itemEffectEntryUis = itemEffectEntryUis;
        }

        public EquipmentType EquipmentType;
        public bool IsEquipped;
        public Rarity ItemRarity;
        public List<ItemEffectEntryUi> itemEffectEntryUis = new List<ItemEffectEntryUi>();
    }


    [Serializable]
    public class ItemEffectEntryUi
    {

        public string effectName;
        public int value;
        public int nextValue;
        public Rarity rarity;
        public RarityPalette rarityPalette;

        public ItemEffectEntryUi(string effectName, int value, int nextValue, Rarity rarity, RarityPalette rarityPalette)
        {
            this.effectName = effectName;
            this.value = value;
            this.nextValue = nextValue;
            this.rarity = rarity;
            this.rarityPalette = rarityPalette;
        }
    }
}