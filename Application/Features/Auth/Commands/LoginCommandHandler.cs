using System.Security.Authentication;
using Application.Exceptions;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using MediatR;

namespace Application.Features.Auth.Commands
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtService jwtService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new InvalidCredentialsException();

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _refreshTokenGenerator.Generate();

            await _userRepository.SaveRefreshTokenAsync(user.UserID, refreshToken.token, refreshToken.expiryTime);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.token,
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}