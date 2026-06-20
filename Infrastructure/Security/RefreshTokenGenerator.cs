using System.Security.Cryptography;
using Application.Interfaces.Services;

namespace Infrastructure.Security
{
    public class RefreshTokenGenerator : IRefreshTokenGenerator
    {
        private const int TokenByteLength = 64;
        private const int TokenLifetimeInDays = 60;

        public (string token, DateTime expiryTime) Generate()
        {
            var randomNumber = new byte[TokenByteLength];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var token = Convert.ToBase64String(randomNumber);
            var expiryTime = DateTime.UtcNow.AddDays(TokenLifetimeInDays);

            return (token, expiryTime);
        }
    }
}