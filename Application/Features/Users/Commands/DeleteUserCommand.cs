using MediatR;

namespace Application.Features.Users.Commands
{
    public class DeleteUserCommand : IRequest
    {
        public int TargetUserId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}