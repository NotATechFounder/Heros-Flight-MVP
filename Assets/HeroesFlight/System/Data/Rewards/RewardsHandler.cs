using System.Collections.Generic;
using System.Linq;
using HeroesFlight.System.FileManager.Model;
using NotImplementedException = System.NotImplementedException;

namespace HeroesFlight.System.FileManager.Rewards
{
    public class RewardsHandler : RewardsHandlerInterface
    {
        List<RewardModel> pendingRewards = new List<RewardModel>();

        public bool RewardPending => pendingRewards.Count > 0;

        public void GrantReward(RewardModel rewardModel)
        {
            if (!pendingRewards.Contains(rewardModel))
            {
                pendingRewards.Add(rewardModel);
            }
        }

        public void ConsumeReward(RewardModel model)
        {
            if (pendingRewards.Contains(model))
            {
                pendingRewards.Remove(model);
            }
        }

        public List<RewardModel> GetPendingRewards()
        {
            return pendingRewards;
        }
    }
}