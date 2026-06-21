using Application.Exceptions;
using Application.Features.Users.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands
{
        public class RegisterCommandHandler : IRequestHandler<RegisterCommand, UserDto>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPasswordHasher _passwordHasher;
            private readonly IMapper _mapper;

            public RegisterCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
            {
                _userRepository = userRepository;
                _passwordHasher = passwordHasher;
                _mapper = mapper;
            }

            public async Task<UserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                if (await _userRepository.IsEmailTakenAsync(request.Email))
                    throw new EmailAlreadyExistsException(request.Email);

                var hashedPassword = _passwordHasher.HashPassword(request.Password);
                var newUser = User.Create(request.UserName, request.Email, hashedPassword);

                await _userRepository.AddUserAsync(newUser);

                return _mapper.Map<UserDto>(newUser);
            }
        }
}

