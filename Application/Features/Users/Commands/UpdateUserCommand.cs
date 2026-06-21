using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class UpdateUserCommand : IRequest<UserDto>
    {
        public int TargetUserId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}