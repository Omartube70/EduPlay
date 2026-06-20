using System.Security.Claims;
using Domain.Entities;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}