using HeroesFlight.System.FileManager.Enum;

namespace HeroesFlight.System.FileManager.Model
{
    public class RewardModel
    {
        public RewardModel(RewardType type)
        {
            RewardType = type;
        }

        public RewardType RewardType { get; }
    }
}