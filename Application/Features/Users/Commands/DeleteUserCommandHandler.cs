using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public DeleteUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId);

            if (user == null)
                throw new UserNotFoundException(request.TargetUserId);

            if (!request.IsAdmin && request.CurrentUserId != request.TargetUserId)
                throw new ForbiddenAccessException("You don't have permission to delete this user.");

            await _userRepository.DeleteUserAsync(request.TargetUserId);
        }
    }
}