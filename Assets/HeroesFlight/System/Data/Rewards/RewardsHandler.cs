
namespace HeroesFlight.System.FileManager.Rewards
{
    public class RewardsHandler :  RewardsHandlerInterface
    {
        public bool RewardPending { get; private set; }
        bool rewardGranted;
        public void GrantReward()
        {
            
            if (rewardGranted)
            {
                return;
            }
            
            RewardPending = true;
        }

        public void ConsumeReward()
        {
            RewardPending = false;
            rewardGranted = true;
        }
    }
}