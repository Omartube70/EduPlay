using Application.Features.Auth.DTOs;
using Application.Interfaces.Repositories;
using MediatR;

namespace Application.Features.Auth.Commands
{
    
        public class RegisterCommand : IRequest<UserDto>
        {
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
}
  