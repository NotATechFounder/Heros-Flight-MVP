using HeroesFlight.System;
using HeroesFlight.System.UI.Reward;
using System.Collections.Generic;

public interface RewardSystemInterface : StateDependantSystemInterface
{
    public void InjectUiConnection();
    void ProcessReward(Reward reward);
    void ProcessRewards(List<Reward> rewards);
    RewardVisual GetRewardVisual(Reward reward);
    List<RewardVisual> GetRewardVisuals(List<Reward> rewards);
    RewardVisual[] GiveLevelUpReward(int gems, int gold);
}