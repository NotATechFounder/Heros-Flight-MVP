using HeroesFlight.System;
using HeroesFlight.System.UI.Reward;
using System.Collections.Generic;

public interface RewardSystemInterface : SystemInterface
{
    public void InjectUiConnection();
    void ProcessReward(Reward reward);
    void ProcessRewards(List<Reward> rewards);
    RewardVisual GetRewardVisual(Reward reward);
    List<RewardVisual> GetRewardVisuals(List<Reward> rewards);
}