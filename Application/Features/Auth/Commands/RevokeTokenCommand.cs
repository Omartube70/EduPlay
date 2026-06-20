using MediatR;

namespace Application.Features.Auth.Commands
{
    public class RevokeTokenCommand : IRequest
    {
        public int TargetUserId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}