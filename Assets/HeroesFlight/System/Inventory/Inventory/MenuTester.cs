using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuTester : MonoBehaviour
{
    [SerializeField] private InventoryMenu inventoryMenu;
    [FormerlySerializedAs("inventorySystem")] [SerializeField] private InventoryHandler inventoryHandler;

    private void Start()
    {
       // inventoryMenu.InitInventory();
      //  inventoryMenu.LoadInventoryItems();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
}
