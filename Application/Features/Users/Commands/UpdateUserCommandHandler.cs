using Application.Exceptions;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId);

            if (user == null)
                throw new UserNotFoundException(request.TargetUserId);

            if (!request.IsAdmin && request.CurrentUserId != request.TargetUserId)
                throw new ForbiddenAccessException("You don't have permission to update this user.");

            // Check email uniqueness if email is being changed
            if (!string.Equals(user.Email, request.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailTaken = await _userRepository.IsEmailTakenAsync(request.Email);
                if (emailTaken)
                    throw new EmailAlreadyExistsException(request.Email);
            }

            user.UpdateInfo(request.UserName, request.Email);

            await _userRepository.UpdateUserAsync(user);

            return _mapper.Map<UserDto>(user);
        }
    }
}