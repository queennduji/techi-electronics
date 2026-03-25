using Techi.Electronics.RewardAPI.Message;

namespace Techi.Electronics.RewardAPI.Services
{
    public interface IRewardService
    {
        Task UpdateRewards(RewardsMessage rewardsMessage);
    }
}
