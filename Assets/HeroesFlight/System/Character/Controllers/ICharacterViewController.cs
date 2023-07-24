using System.Collections.Generic;
using HeroesFlight.Common;
using HeroesFlight.System.Character.Enum;

namespace HeroesFlight.System.Character
{
    public interface ICharacterViewController
    {
        void SetupView(AppearanceData data);
        void Equip(string itemSkin, ItemVisualType itemType);
    }
}