using Application.Exceptions;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Application.Interfaces.Repositories;
using AutoMapper;
using MediatR;

namespace Application.Features.Users.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByIdAsync(request.TargetUserId);

            if (user == null)
                throw new UserNotFoundException(request.TargetUserId);

            if (!request.IsAdmin && request.CurrentUserId != request.TargetUserId)
                throw new ForbiddenAccessException("You don't have permission to view this user's information.");

            return _mapper.Map<UserDto>(user);
        }
    }
}