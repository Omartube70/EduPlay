using Application.Features.Auth.DTOs;
using Application.Interfaces.Repositories;

namespace Application.Features.Auth.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserDto User { get; set; } = new();
    }
}
