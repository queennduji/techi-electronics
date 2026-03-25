using Techi.Electronics.EmailAPI.Data.Model.Dto;
using Techi.Electronics.EmailAPI.Message;

namespace Techi.Electronics.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
