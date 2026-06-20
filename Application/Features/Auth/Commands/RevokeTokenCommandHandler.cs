using Application.Exceptions;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands
{
    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand>
    {
        private readonly IUserRepository _userRepository;

        public RevokeTokenCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        {
            bool isRevokingSelf = request.CurrentUserId == request.TargetUserId;

            if (!request.IsAdmin && !isRevokingSelf)
                throw new ForbiddenAccessException();

            await _userRepository.SaveRefreshTokenAsync(request.TargetUserId, null, null);
        }
    }
}