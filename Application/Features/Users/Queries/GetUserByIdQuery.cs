
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public int TargetUserId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}