using System;
using System.Collections.Generic;
using HeroesFlight.Common.Enum;
using HeroesFlight.System.Combat.Effects.Effects;
using HeroesFlight.System.Combat.Effects.Effects.Data;
using HeroesFlight.System.Inventory.Inventory.Converter;
using HeroesFlight.System.UI;
using HeroesFlight.System.UI.Inventory_Menu;
using StansAssets.Foundation.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HeroesFlight.System.Inventory
{
    public class InventorySystem : InventorySystemInterface

    {
        public InventorySystem(DataSystemInterface dataSystemInterface, IUISystem uiSystem)
        {
            data = dataSystemInterface;
            this.uiSystem = uiSystem;
        }

        private DataSystemInterface data;
        private IUISystem uiSystem;
        private InventoryItemConverter converter;

        public void Init(Scene scene = default, Action onComplete = null)
        {
            InventoryHandler = scene.GetComponent<InventoryHandler>();
            InventoryHandler.Init(data.CurrencyManager);
            InventoryHandler.OnItemAdded += SpawnUiItem;
            InventoryHandler.OnItemModified += UpdateUiItem;
            converter = new InventoryItemConverter(InventoryHandler);
        }

        public void Reset()
        {
            //TODO: reset callback if needed 
        }

        private void HandleItemUnequipRequest(EquipmentEntryUi obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.InstanceId);
            InventoryHandler.UnEquipItem(targetItem);
            //  uiSystem.UiEventHandler.InventoryMenu.UnEquipItem();
            UpdateInventoryUi();
        }

        private void HandleItemDismantleRequest(EquipmentEntryUi obj)
        {
            Debug.Log($"dismantling {obj.ID}");
            Item targetItem = InventoryHandler.GetEqupItemById(obj.InstanceId);
            InventoryHandler.DismantleItem(targetItem);
            uiSystem.UiEventHandler.InventoryMenu.DismantleItem();
            UpdateInventoryUi();
        }

        private void HandleItemUpgradeRequest(EquipmentEntryUi obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.InstanceId);
            if (InventoryHandler.TryUpgradeItem(targetItem))
            {
                uiSystem.UiEventHandler.InventoryMenu.UpgradeItem();
                UpdateInventoryUi();
            }
        }

        private void HandleItemEquipRequest(EquipmentEntryUi obj)
        {
            Debug.Log(obj);
            Item targetItem = InventoryHandler.GetEqupItemById(obj.InstanceId);
            InventoryHandler.EquipItem(targetItem);
            UpdateInventoryUi();
        }

        private void OpenInventory()
        {
            UpdateInventoryUi();
            uiSystem.UiEventHandler.InventoryMenu.Open();
        }

        private void UpdateInventoryUi()
        {
            List<InventoryItemUiEntry> materials = new();
            List<EquipmentEntryUi> equipment = new();
            //Debug.Log(InventoryHandler.GetInventoryEquippmentItems().Count);
            foreach (var equipmentItem in InventoryHandler.GetInventoryEquippmentItems())
            {
                var equipmentData = equipmentItem.GetItemData<ItemEquipmentData>();
                equipment.Add(new EquipmentEntryUi(equipmentItem.GetItemData<ItemEquipmentData>().instanceID, equipmentItem.itemSO.icon,
                    equipmentData.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description,
                    (equipmentItem.itemSO as EquipmentSO).equipmentType,
                    equipmentData.eqquiped, equipmentData.rarity, equipmentData.instanceID));
            }

            foreach (var equipmentItem in InventoryHandler.GetInventoryMaterialItems())
            {
                var itemData = equipmentItem.GetItemData<ItemMaterialData>();
                materials.Add(new InventoryItemUiEntry(equipmentItem.itemSO.ID, equipmentItem.itemSO.icon,
                    itemData.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description));
            }

            uiSystem.UiEventHandler.InventoryMenu.UpdateInventoryView(equipment, materials);
        }

        private void UpdateUiItem(Item obj)
        {
            UpdateInventoryUi();
            // if (obj.itemSO.itemType == ItemType.Material)
            // {
            //     var itemData = obj.GetItemData<ItemMaterialData>();
            //
            //     uiSystem.UiEventHandler.InventoryMenu.UpdateItemUI(new InventoryItemUiEntry(obj.itemSO.ID,
            //         obj.itemSO.icon, itemData.value,
            //         obj.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
            //         obj.itemSO.Name, obj.itemSO.description));
            // }
            // else
            // {
            //     var equipmentData = obj.GetItemData<ItemEquipmentData>();
            //     uiSystem.UiEventHandler.InventoryMenu.UpdateItemUI(new EquipmentEntryUi(obj.itemSO.ID, obj.itemSO.icon,
            //         equipmentData.value,
            //         obj.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
            //         obj.itemSO.Name, obj.itemSO.description,
            //         (obj.itemSO as EquipmentSO).equipmentType,
            //         equipmentData.eqquiped, equipmentData.rarity,equipmentData.instanceID));
            // }
        }

        private void SpawnUiItem(Item obj)
        {
            if (obj.itemSO.itemType == ItemType.Material)
            {
                var itemData = obj.GetItemData<ItemMaterialData>();
                uiSystem.UiEventHandler.InventoryMenu.SpawnItemUI((new InventoryItemUiEntry(obj.itemSO.ID,
                    obj.itemSO.icon, itemData.value,
                    obj.itemSO.itemType, InventoryHandler.GetPalette(Rarity.Common),
                    obj.itemSO.Name, obj.itemSO.description)));
            }
            else
            {
                var equipmentData = obj.GetItemData<ItemEquipmentData>();
                uiSystem.UiEventHandler.InventoryMenu.SpawnItemUI(new EquipmentEntryUi(obj.itemSO.ID, obj.itemSO.icon,
                    equipmentData.value,
                    obj.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
                    obj.itemSO.Name, obj.itemSO.description,
                    (obj.itemSO as EquipmentSO).equipmentType,
                    equipmentData.eqquiped, equipmentData.rarity, equipmentData.instanceID));
            }

            UpdateInventoryUi();
        }

        public InventoryHandler InventoryHandler { get; private set; }

        public void InjectUiConnection()
        {
            uiSystem.UiEventHandler.MainMenu.OnInventoryButtonPressed += OpenInventory;
            uiSystem.UiEventHandler.InventoryMenu.OnEquipItemRequest += HandleItemEquipRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUpgradeRequest += HandleItemUpgradeRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnDismantleRequest += HandleItemDismantleRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUnEquipItemRequest += HandleItemUnequipRequest;

            uiSystem.UiEventHandler.InventoryMenu.InitInventory(converter);
            
            Debug.Log("SUBSCRIBED");
        }

        public List<CombatEffect> GetEquippedItemsCombatEffects()
        {
            List<Item> items = InventoryHandler.GetInventoryEquippedItems();
            List<CombatEffect> combatEffects = new List<CombatEffect>();
            foreach (var item in items)
            {
                EquipmentSO equipmentSO = item.itemSO as EquipmentSO;
                foreach (UniqueCombatEffect uniqueCombatEffect in equipmentSO.uniqueCombatEffects)
                {
                    CombatEffect combatEffectInstance = uniqueCombatEffect.combatEffect.Clone();
                    combatEffects.Add(combatEffectInstance);

                    foreach (Effect effets in uniqueCombatEffect.combatEffect.EffectToApply)
                    {
                        switch (effets)
                        {
                            case BurnStatusEffect:
                                {
                                    BurnStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as BurnStatusEffect;
                                    effectInstance.GetData<BurnEffectData>().Damage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance); 
                                }
                                break;

                            case FreezeStatusEffect:
                                {
                                    FreezeStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as FreezeStatusEffect;
                                    effectInstance.GetData<FreezeEffectData>().SlowAmount.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case PoisonStatusEffect:
                                {
                                    PoisonStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as PoisonStatusEffect;
                                    effectInstance.GetData<PoisonEffectData>().Damage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case RootStatusEffect:
                                {
                                    RootStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as RootStatusEffect;
                                    effectInstance.GetData<RootEffectData>().Damage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case ShockStatusEffect:
                                {
                                    ShockStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as ShockStatusEffect;
                                    effectInstance.GetData<ShockEffectData>().MainDamage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    effectInstance.GetData<ShockEffectData>().SecondaryDamage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;
                            default:  break;
                        }
                    }
                }
            }
            return combatEffects;
        }
    }
}