using System;
using HeroesFlight.System.Cheats.Data;
using HeroesFlight.System.Cheats.Enum;
using HeroesFlight.System.Cheats.UI;
using HeroesFlight.System.Combat;
using HeroesFlight.System.Gameplay;
using HeroesFlight.System.Inventory;
using HeroesFlight.System.NPC;
using HeroesFlight.System.Stats.Handlers;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Cheats
{
    public class CheatSystem : CheatSystemInterface
    {
        public CheatSystem(GamePlaySystemInterface gamePlaySystemInterface, DataSystemInterface dataSystemInterface,
            CombatSystemInterface combatSystemInterface, TraitSystemInterface traitSystemInterface,
            NpcSystemInterface npcSystemInterface,InventorySystemInterface inventorySystemInterface)
        {
            gamePlaySystem = gamePlaySystemInterface;
            dataSystem = dataSystemInterface;
            combatSystem = combatSystemInterface;
            traitSystem = traitSystemInterface;
            npcSystem = npcSystemInterface;
            inventorySystem = inventorySystemInterface;
            profile = Resources.Load<CheatsDataProfile>(Setup_File_Location);
        }

        GamePlaySystemInterface gamePlaySystem;
        DataSystemInterface dataSystem;
        CombatSystemInterface combatSystem;
        TraitSystemInterface traitSystem;
        NpcSystemInterface npcSystem;
        InventorySystemInterface inventorySystem;

        private const string GOLD_CURRENCY_KEY = "GP";
        private const string GEMS_CURRENCY_KEY = "GEM";

        private const string Setup_File_Location = "Cheats/CheatsDataProfile";
        private CheatsDataProfile profile;
        private CheatsUiControllerInterface uiController;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            uiController = scene.GetComponentInChildren<CheatsUiController>();
            uiController.SetState(profile.EnableCheats);
            uiController.OnCheatButtonClicked += HandleCheatButtonClicked;
        }

        public void Reset() { }

        /// <summary>
        /// Adds currency to the currency manager.
        /// </summary>
        public void AddCurrency()
        {
            dataSystem.CurrencyManager.AddCurrency(GEMS_CURRENCY_KEY,10000);
            dataSystem.CurrencyManager.AddCurrency(GOLD_CURRENCY_KEY,10000);
        }

        /// <summary>
        /// Adds predefined items to the inventory system.
        /// </summary>
        public void AddItems()
        {
            inventorySystem.AddPredefinedItems();
        }

        /// <summary>
        /// Unlocks all traits for the user.
        /// </summary>
        public void UnlockTraits()
        {
            traitSystem.UnlockAllTraits();
        }

        /// <summary>
        /// Kills all enemies in the game.
        /// </summary>
        public void KillAllEnemies()
        {
            npcSystem.KillAllSpawnedEntities();
        }

        /// <summary>
        /// Makes the player immortal.
        /// </summary>
        public void MakePlayerImmortal(bool isImmortal)
        {
            combatSystem.MakePlayerImmortal(isImmortal);
        }

        /// <summary>
        /// Handles the click event of the cheat button.
        /// </summary>
        /// <param name="eventData">The cheat button click model.</param>
        void HandleCheatButtonClicked(CheatButtonClickModel eventData)
        {
            Debug.Log(eventData.ButtonType);
            switch (eventData.ButtonType)
            {
                case CheatsButtonType.AddCurrency:
                    AddCurrency();
                    break;
                case CheatsButtonType.UnlockTraits:
                    UnlockTraits();
                    break;
                case CheatsButtonType.Immortality:
                    MakePlayerImmortal(eventData.ToggleValue);
                    break;
                case CheatsButtonType.AddItems:
                    AddItems();
                    break;
                case CheatsButtonType.KillAllMobs:
                    KillAllEnemies();
                    break;
                case CheatsButtonType.Navigation:
                    break;
               
            }
        }
    }
}