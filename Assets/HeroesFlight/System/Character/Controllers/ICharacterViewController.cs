using System.Collections.Generic;
using HeroesFlight.System.Character.Enum;

namespace HeroesFlight.System.Character
{
    public interface ICharacterViewController
    {
        void SetupView(Dictionary<ItemVisualType,string> viewParts);
        void Equip(string itemSkin, ItemVisualType itemType);
    }
}