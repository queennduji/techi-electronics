using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Techi.Electronics.AuthAPI.Data;
using Techi.Electronics.AuthAPI.Models;
using Techi.Electronics.AuthAPI.Models.Dto;
using Techi.Electronics.AuthAPI.Service.IService;

namespace Techi.Electronics.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var normalizedEmail = email.ToLower();

            var user = await _db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Email!.ToLower() == normalizedEmail, cancellationToken);

            if (user == null)
            {
                return false;
            }

            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var createRoleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (!createRoleResult.Succeeded)
                {
                    return false;
                }
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
            return addToRoleResult.Succeeded;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
        {
            var normalizedUserName = loginRequestDto.UserName.ToLower();

            var user = await _db.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName!.ToLower() == normalizedUserName, cancellationToken);

            if (user == null)
            {
                return new LoginResponseDto
                {
                    User = null,
                    Token = string.Empty
                };
            }

            cancellationToken.ThrowIfCancellationRequested();

            var isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isValid)
            {
                return new LoginResponseDto
                {
                    User = null,
                    Token = string.Empty
                };
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            var userDto = new UserDto
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            return new LoginResponseDto
            {
                User = userDto,
                Token = token
            };

        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var user = new ApplicationUser
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);

                if (!result.Succeeded)
                {
                    return result.Errors.FirstOrDefault()?.Description ?? "Error encountered";
                }

                var userToReturn = await _db.ApplicationUsers
                    .FirstAsync(u => u.UserName == registrationRequestDto.Email, cancellationToken);

                return string.Empty;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return "Error encountered";
            }
        }
    }
}
