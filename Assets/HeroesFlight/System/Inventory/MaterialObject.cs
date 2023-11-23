using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Material", menuName = "Inventory System/Items/Material")]
public class MaterialObject : ItemSO
{
    public int maxStackAmount;
    public EquipmentType equipmentType;

    private void Awake() => itemType = ItemType.Material;
}
