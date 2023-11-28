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
        inventoryMenu.InitInventory(inventorySystem);
        inventoryMenu.LoadInventoryItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
