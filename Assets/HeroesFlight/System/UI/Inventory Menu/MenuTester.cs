using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class MenuTester : MonoBehaviour
{
    [SerializeField] private InventoryMenu inventoryMenu;
    [SerializeField] private InventorySystem inventorySystem;

    private void Start()
    {
        inventorySystem.OnItemAdded += inventoryMenu.SpawnItemUI;
        inventorySystem.OnItemModified += inventoryMenu.UpdateItemUI;

        inventoryMenu.GetInventoryItems += inventorySystem.GetInventoryItems;
        inventoryMenu.OnItemEquipped += inventorySystem.EquipItem;
        inventoryMenu.OnItemUnEquipped += inventorySystem.UnEquipItem;
        inventoryMenu.OnItemDismantled += inventorySystem.DismantleItem;
        inventoryMenu.OnItemUpgraded += inventorySystem.TryUpgradeItem;

        inventoryMenu.InitInventoryUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inventoryMenu.LoadInventoryItems();
        }
    }
}
