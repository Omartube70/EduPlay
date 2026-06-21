using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class PromoteToAdminCommand : IRequest<UserDto>
    {
        public int TargetUserId { get; set; }
    }
}