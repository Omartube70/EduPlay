using Application.Exceptions;
using Application.Features.Users.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Users.Commands
{
    public class PromoteToAdminCommandHandler : IRequestHandler<PromoteToAdminCommand, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public PromoteToAdminCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(PromoteToAdminCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId);

            if (user == null)
                throw new UserNotFoundException(request.TargetUserId);

            user.PromoteToAdmin();

            await _userRepository.UpdateUserAsync(user);

            return _mapper.Map<UserDto>(user);
        }
    }
}