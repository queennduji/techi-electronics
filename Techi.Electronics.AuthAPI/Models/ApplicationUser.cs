using Microsoft.AspNetCore.Identity;

namespace Techi.Electronics.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
