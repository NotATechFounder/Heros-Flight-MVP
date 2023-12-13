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
            //InventoryHandler.OnItemAdded += SpawnUiItem;
           // InventoryHandler.OnItemModified += UpdateUiItem;
            InventoryHandler.OnInventoryUpdated += UpdateInventoryUi;
            converter = new InventoryItemConverter(InventoryHandler);
            
            InventoryHandler.OnEqiuppedItemsStatChanged += data.StatManager.ProcessEquippedItemsModifiers;
        }

        public void Reset()
        {
            //TODO: reset callback if needed 
        }

        private void HandleItemUnequipRequest(EquipmentEntryUi obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            InventoryHandler.UnEquipItem(targetItem);
            //  uiSystem.UiEventHandler.InventoryMenu.UnEquipItem();
            UpdateInventoryUi();
        }

        private void HandleItemDismantleRequest(EquipmentEntryUi obj)
        {
            Debug.Log($"dismantling {obj.ID}");
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            InventoryHandler.DismantleItem(targetItem);
            uiSystem.UiEventHandler.InventoryMenu.DismantleItem();
            UpdateInventoryUi();
        }

        private void HandleItemUpgradeRequest(EquipmentEntryUi obj)
        {
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            if (InventoryHandler.TryUpgradeItem(targetItem))
            {
                UpdateInventoryUi();
                uiSystem.UiEventHandler.InventoryMenu.UpgradeItem();
            }
        }

        private void HandleItemEquipRequest(EquipmentEntryUi obj)
        {
            Debug.Log(obj);
            Item targetItem = InventoryHandler.GetEqupItemById(obj.ID);
            InventoryHandler.EquipItem(targetItem);
            UpdateInventoryUi();
        }

        private void OpenInventory()
        {
            UpdateInventoryUi();
            if(!uiSystem.UiEventHandler.InventoryMenu.isActiveAndEnabled)
             uiSystem.UiEventHandler.InventoryMenu.Open();
        }

        private void UpdateInventoryUi()
        {
            List<InventoryItemUiEntry> materials = new();
            List<EquipmentEntryUi> equipment = new();

            foreach (var equipmentItem in InventoryHandler.GetInventoryEquippmentItems())
            {
                ItemEquipmentData equipmentData = equipmentItem.GetItemData<ItemEquipmentData>();

                List<ItemEffectEntryUi> itemEffectEntryUis = new List<ItemEffectEntryUi>();
                EquipmentSO equipmentSO = equipmentItem.GetItemSO<EquipmentSO>();

                itemEffectEntryUis.Add(new ItemEffectEntryUi(equipmentSO.statType.ToString(), InventoryHandler.GetItemCurrentStat(equipmentItem, equipmentData.value), InventoryHandler.GetItemCurrentStat(equipmentItem, equipmentData.value + 1), Rarity.Common, InventoryHandler.GetPalette(Rarity.Common)));
                itemEffectEntryUis.Add(new ItemEffectEntryUi(equipmentSO.specialHeroEffect.statType.ToString(), equipmentSO.specialHeroEffect.value,0, Rarity.Common, InventoryHandler.GetPalette(Rarity.Common)));

                foreach (var effect in equipmentSO.uniqueStatModificationEffects)
                {
                    itemEffectEntryUis.Add(new ItemEffectEntryUi(effect.statType.ToString(), effect.curve.GetCurrentValueInt(equipmentData.value), effect.curve.GetCurrentValueInt(equipmentData.value + 1), effect.rarity, InventoryHandler.GetPalette(effect.rarity)));
                }

                foreach (var effect in equipmentSO.uniqueCombatEffects)
                {
                    itemEffectEntryUis.Add(new ItemEffectEntryUi(effect.combatEffect.EffectToApply[0].name, effect.curve.GetCurrentValueInt(equipmentData.value), effect.curve.GetCurrentValueInt(equipmentData.value + 1), effect.rarity, InventoryHandler.GetPalette(effect.rarity)));
                }

                equipment.Add(new EquipmentEntryUi(equipmentData.instanceID, equipmentItem.itemSO.icon,
                    equipmentData.value,
                    equipmentItem.itemSO.itemType, InventoryHandler.GetPalette(equipmentData.rarity),
                    equipmentItem.itemSO.Name, equipmentItem.itemSO.description,
                    (equipmentItem.itemSO as EquipmentSO).equipmentType,
                    equipmentData.eqquiped, equipmentData.rarity, itemEffectEntryUis));
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

        public InventoryHandler InventoryHandler { get; private set; }

        public void InjectUiConnection()
        {
            uiSystem.UiEventHandler.MainMenu.OnNavigationButtonClicked += MainMenu_OnNavigationButtonClicked;
            uiSystem.UiEventHandler.InventoryMenu.OnEquipItemRequest += HandleItemEquipRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUpgradeRequest += HandleItemUpgradeRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnDismantleRequest += HandleItemDismantleRequest;
            uiSystem.UiEventHandler.InventoryMenu.OnUnEquipItemRequest += HandleItemUnequipRequest;

            uiSystem.UiEventHandler.InventoryMenu.InitInventory(converter);
            
            //Debug.Log("SUBSCRIBED");
        }

        private void MainMenu_OnNavigationButtonClicked(UISystem.MenuNavigationButtonType obj)
        {
            if (obj == UISystem.MenuNavigationButtonType.Inventory)
            {
                OpenInventory();
            }
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
                    if (uniqueCombatEffect.rarity > item.GetItemData<ItemEquipmentData>().rarity) continue;

                    CombatEffect combatEffectInstance = uniqueCombatEffect.combatEffect.Clone();
                    combatEffects.Add(combatEffectInstance);

                    foreach (Effect effets in uniqueCombatEffect.combatEffect.EffectToApply)
                    {
                        switch (effets)
                        {
                            case BurnStatusEffect:
                                {
                                    BurnStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as BurnStatusEffect;
                                    effectInstance.SetData(effets as BurnStatusEffect);
                                    BurnEffectData burnEffectData = effectInstance.GetData<BurnEffectData>();
                                    burnEffectData.ProcChance.SetStartValue(effets.GetData<BurnEffectData>().ProcChance.StartValue);
                                    burnEffectData.Damage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case FreezeStatusEffect:
                                {
                                    FreezeStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as FreezeStatusEffect;
                                    effectInstance.SetData(effets as FreezeStatusEffect);
                                    FreezeEffectData freezeEffectData = effectInstance.GetData<FreezeEffectData>();
                                    freezeEffectData.ProcChance.SetStartValue(effets.GetData<FreezeEffectData>().ProcChance.StartValue);
                                    freezeEffectData.SlowAmount.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case PoisonStatusEffect:
                                {
                                    PoisonStatusEffect effectInstance = GameObject.Instantiate(effets) as PoisonStatusEffect;
                                    PoisonEffectData poisonEffectData = effectInstance.GetData<PoisonEffectData>();
                                    poisonEffectData.ProcChance.SetStartValue(effets.GetData<PoisonEffectData>().ProcChance.StartValue);
                                    poisonEffectData.Damage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case RootStatusEffect:
                                {
                                    RootStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as RootStatusEffect;
                                    effectInstance.SetData(effets as RootStatusEffect);
                                    RootEffectData rootEffectData = effectInstance.GetData<RootEffectData>();
                                    rootEffectData.ProcChance.SetStartValue(effets.GetData<RootEffectData>().ProcChance.StartValue);
                                    rootEffectData.Damage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case ShockStatusEffect:
                                {
                                    ShockStatusEffect effectInstance = ScriptableObject.CreateInstance(effets.GetType()) as ShockStatusEffect;
                                    effectInstance.SetData(effets as ShockStatusEffect);
                                    ShockEffectData shockEffectData = effectInstance.GetData<ShockEffectData>();    
                                    shockEffectData.ProcChance.SetStartValue(effets.GetData<ShockEffectData>().ProcChance.StartValue);
                                    shockEffectData.MainDamage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    shockEffectData.SecondaryDamage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
                                    combatEffectInstance.EffectToApply.Add(effectInstance);
                                }
                                break;

                            case ReflectTriggerEffect:
                                {
                                    ReflectTriggerEffect effectInstance = GameObject.Instantiate(effets) as ReflectTriggerEffect;
                                    ReflectEffectData reflectEffectData = effectInstance.GetData<ReflectEffectData>();
                                    reflectEffectData.FlatDamage.SetStartValue(uniqueCombatEffect.curve.GetCurrentValueInt(item.GetItemData<ItemEquipmentData>().value));
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