using System.Collections.Generic;
using HeroesFlight.System.FileManager.Enum;
using HeroesFlight.System.FileManager.Model;
using UnityEngine;

namespace HeroesFlight.System.FileManager.Rewards
{
    public class RewardsHandler : RewardsHandlerInterface
    {
        //    Dictionary<RewardType,List<RewardModel>> pendingRewards = new ();
        //    Dictionary<RewardType,List<RewardModel>> receivedRewards = new();
        //    public bool RewardPending => pendingRewards.Count > 0;

        //    public void GrantReward(RewardModel rewardModel)
        //    {
        //        if (rewardModel.RewardType == RewardType.Hero)
        //        {
        //            var model = rewardModel as HeroRewardModel;
        //            var alreadyReceived = false;
        //            if (receivedRewards.TryGetValue(rewardModel.RewardType, out var alreadyGrantedRewards))
        //            {
        //                foreach (var reward in alreadyGrantedRewards)
        //                {
        //                    var currentReward = reward as HeroRewardModel;
        //                    if (currentReward.HeroType == model.HeroType)
        //                    {
        //                        Debug.Log("Thsi rewards is already granted");
        //                        alreadyReceived = true;
        //                        break;
        //                    }
        //                }
        //            }


        //            if (alreadyReceived)
        //                return;

        //            var alreadyPending = false;

        //            if (pendingRewards.TryGetValue(rewardModel.RewardType, out var alreadyPendingRewards))
        //            {
        //                foreach (var reward in alreadyPendingRewards)
        //                {
        //                    var currentReward = reward as HeroRewardModel;
        //                    if (currentReward.HeroType == model.HeroType)
        //                    {
        //                        Debug.Log("Thsi rewards is already pending");
        //                        alreadyPending = true;
        //                        break;
        //                    }
        //                }
        //            }


        //            if (alreadyPending)
        //                return;

        //            Debug.Log("granting reward");
        //            if (pendingRewards.TryGetValue(model.RewardType, out var rewards))
        //            {
        //                rewards.Add(model);
        //            }
        //            else
        //            {
        //                pendingRewards.Add(model.RewardType,new List<RewardModel>(){model});
        //            }

        //        }

        //    }

        //    public void ConsumeReward(RewardModel rewardModel)
        //    {
        //        if (rewardModel.RewardType == RewardType.Hero)
        //        {
        //            var model = rewardModel as HeroRewardModel;

        //            if (pendingRewards.TryGetValue(rewardModel.RewardType, out var rewards))
        //            {
        //                RewardModel modelToRemove = null;
        //                foreach (var reward in rewards)
        //                {
        //                    var heroReward = reward as HeroRewardModel;
        //                    if (heroReward.HeroType == model.HeroType)
        //                    {
        //                        modelToRemove = heroReward;
        //                        break;
        //                    }
        //                }

        //                if (modelToRemove != null)
        //                {

        //                    Debug.Log("Consuming reward");
        //                    rewards.Remove(modelToRemove);

        //                    if (receivedRewards.TryGetValue(modelToRemove.RewardType, out var grantedRewards))
        //                    {
        //                        grantedRewards.Add(modelToRemove);
        //                    }
        //                    else
        //                    {
        //                        receivedRewards.Add(modelToRemove.RewardType,new List<RewardModel>(){modelToRemove});
        //                    }
        //                }



        //            }


        //        }
        //    }

        //    public Dictionary<RewardType,List<RewardModel>> GetPendingRewards()
        //    {
        //        return pendingRewards;
        //    }
        public bool RewardPending => throw new global::System.NotImplementedException();

        public void ConsumeReward(RewardModel model)
        {
            throw new global::System.NotImplementedException();
        }

        public Dictionary<RewardType, List<RewardModel>> GetPendingRewards()
        {
            throw new global::System.NotImplementedException();
        }

        public void GrantReward(RewardModel rewardModel)
        {
            throw new global::System.NotImplementedException();
        }
    }
}