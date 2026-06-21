using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId);

            if (user == null)
                throw new UserNotFoundException(request.TargetUserId);


            if (!request.IsAdmin && !(request.CurrentUserId == request.TargetUserId))
                throw new ForbiddenAccessException("You don't have permission to change this user's password.");


            var newHash = _passwordHasher.HashPassword(request.NewPassword);
            user.ChangePassword(newHash);

            await _userRepository.UpdateUserAsync(user);
        }
    }
}