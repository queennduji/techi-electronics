namespace Techi.Electronics.AuthAPI.Models.Dto
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }
        public String Token { get; set; }
    }
}
