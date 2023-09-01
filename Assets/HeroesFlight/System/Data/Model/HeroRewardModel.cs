using HeroesFlight.Common.Enum;
using HeroesFlight.System.FileManager.Enum;

namespace HeroesFlight.System.FileManager.Model
{
    public class HeroRewardModel : RewardModel
    {
        public HeroRewardModel(RewardType type,CharacterType heroType) : base(type)
        {
            HeroType = heroType;
        }

        public CharacterType HeroType { get; }
    }
}