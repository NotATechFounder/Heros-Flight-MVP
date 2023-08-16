namespace HeroesFlight.System.FileManager.Rewards
{
    public interface RewardsHandlerInterface
    {
        bool RewardPending { get; }
        void GrantReward();
        void ConsumeReward();
    }
}