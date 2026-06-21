using Application.Exceptions;
using Application.Features.Auth.DTOs;
using Application.Features.Users.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using MediatR;

namespace Application.Features.Auth.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            IJwtService jwtService,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mapper = mapper;
        }

        public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync(request.RefreshToken);

            if (user == null)
                throw new InvalidTokenException("Invalid or expired refresh token.");

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshToken = _refreshTokenGenerator.Generate();

            await _userRepository.SaveRefreshTokenAsync(user.UserID, newRefreshToken.token, newRefreshToken.expiryTime);

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.token,
                User = _mapper.Map<UserDto>(user)
            };
        }
    }
}