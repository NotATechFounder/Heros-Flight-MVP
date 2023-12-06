
using HeroesFlight.System.Combat.Effects.Effects;
using System.Collections.Generic;

namespace HeroesFlight.System.Inventory
{
    public interface InventorySystemInterface : SystemInterface
    {
        InventoryHandler InventoryHandler { get; }
        void InjectUiConnection();

        List<CombatEffect> GetEquippedItemsCombatEffects();
    }
}