using MediatR;

namespace Application.Features.Users.Commands
{
    public class ChangePasswordCommand : IRequest
    {
        public int TargetUserId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsAdmin { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}