using System.Collections.Generic;
using HeroesFlight.System.FileManager.Model;

namespace HeroesFlight.System.FileManager.Rewards
{
    public interface RewardsHandlerInterface
    {
        bool RewardPending { get; }
        void GrantReward(RewardModel rewardModel);
        void ConsumeReward(RewardModel model);
        List<RewardModel> GetPendingRewards();
    }
}