using System;
using HeroesFlight.Common;
using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlight.System.UI.Inventory_Menu
{
    [Serializable]
    public class InventoryItemUiEntry
    {
        public InventoryItemUiEntry(string id, Sprite icon, int value,ItemType itemType, RarityPalette rarityPallete, string name, string description)
        {
            ID = id;
            Icon = icon;
            Value = value;
            ItemType = itemType;
            RarityPallete = rarityPallete;
            Name = name;
            Description = description;
        }
        public string ID;
        public string Name;
        public string Description;
        public Sprite Icon;
        public int Value;
        public ItemType ItemType;
        public RarityPalette RarityPallete;

    }
}