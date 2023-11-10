using HeroesFlight.Common.Enum;
using UnityEngine;

namespace HeroesFlight.Common.Feat
{
    public class FeatUiViewData
    {
        public FeatUiViewData(string id, Sprite icon, string description,FeatState state)
        {
            ID = id;
            Icon = icon;
            Description = description;
            State = state;
        }
        public string ID { get; }
        public Sprite Icon { get; }
        public string Description { get; }
        public FeatState State { get; }

    }
}