using Techi.Electronics.AuthAPI.Models;

namespace Techi.Electronics.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles);
    }
}
