using System.Collections;
using System.Collections.Generic;
using UISystem;
using UnityEngine;

public class MenuTester : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private InventoryMenu inventoryMenu;
    [SerializeField] private CharacterSelectMenu characterSelectMenu;


    private void Start()
    {
        //inventoryMenu.GetSelectedCharacterSO += characterManager.GetSelectedCharacter;
        //inventoryMenu.OnChangeHeroButtonClicked += characterSelectMenu.Open;

        characterSelectMenu.OnCharacterSelected += characterManager.ToggleCharacterSelected;

        //inventoryMenu.OnCreated();
        characterSelectMenu.OnCreated();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryMenu.OnOpened();
        }
    }
}
