using Application.Features.Auth.DTOs;
using MediatR;

namespace Application.Features.Auth.Commands
{
    public class RefreshTokenCommand : IRequest<LoginResponseDto>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}