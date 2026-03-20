using Techi.Electronics.EmailAPI.Data.Model.Dto;

namespace Techi.Electronics.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
    }
}
