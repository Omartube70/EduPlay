using Application.Features.Users.DTOs;
using MediatR;

namespace Application.Features.Users.Queries
{
    public class GetAllUsersQuery : IRequest<IReadOnlyList<UserDto>>
    {
    }
}