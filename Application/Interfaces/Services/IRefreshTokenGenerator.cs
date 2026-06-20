namespace Application.Interfaces.Services
{
    public interface IRefreshTokenGenerator
    {
        (string token, DateTime expiryTime) Generate();
    }
}