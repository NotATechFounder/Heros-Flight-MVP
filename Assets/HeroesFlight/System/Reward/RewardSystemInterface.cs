using HeroesFlight.System;
using System.Collections.Generic;

public interface RewardSystemInterface : SystemInterface
{
    public void InjectUiConnection();
    void ProcessReward(Reward reward);
    void ProcessRewards(List<Reward> rewards);
}