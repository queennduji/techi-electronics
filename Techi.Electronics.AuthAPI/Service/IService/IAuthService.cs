using Techi.Electronics.AuthAPI.Models.Dto;

namespace Techi.Electronics.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto, CancellationToken cancellationToken);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken);

        Task<bool> AssignRole(string email, string roleName, CancellationToken cancellationToken);
    }
}
