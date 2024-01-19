using HeroesFlight.System;
using HeroesFlight.System.UI.Reward;
using System.Collections.Generic;

public interface RewardSystemInterface : StateDependantSystemInterface
{
    public void InjectUiConnection();
    void ProcessReward(Reward reward);
    void ProcessRewards(List<Reward> rewards);
    RewardVisualEntry GetRewardVisual(Reward reward);
    List<RewardVisualEntry> GetRewardVisuals(List<Reward> rewards);
    RewardVisualEntry[] GiveLevelUpReward(int gems, int gold);
}