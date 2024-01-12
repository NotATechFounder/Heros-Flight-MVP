using HeroesFlight.System.UI.Enum;

namespace HeroesFlight.System.UI.Model
{
    public class ReviveRequestModel
    {
        public ReviveRequestModel(UiReviveType reviveType, int cost)
        {
            ReviveType = reviveType;
            Cost = cost;
        }

       
        public UiReviveType ReviveType { get; }
        public int Cost { get; }
    }
}