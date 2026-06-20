using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public int UserID { get; private set; }
        public string UserName { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Permissions UserPermissions { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        public ICollection<Document> Documents { get; private set; } = new List<Document>();

#pragma warning disable CS8618
        private User() { }
#pragma warning restore CS8618

        private User(string userName, string email, string passwordHash)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            UserPermissions = Permissions.User;
        }

        public static User Create(string userName, string email, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("Username is required.", nameof(userName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash is required.", nameof(passwordHash));

            return new User(userName, email, passwordHash);
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty.");

            PasswordHash = newPasswordHash;
        }

        public void SetRefreshToken(string? refreshToken, DateTime? expiryTime)
        {
            if (refreshToken != null && expiryTime.HasValue && expiryTime.Value < DateTime.UtcNow)
                throw new ArgumentException("Expiry time must be in the future.");

            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void PromoteToAdmin()
        {
            if (UserPermissions == Permissions.Admin)
                return;

            UserPermissions = Permissions.Admin;
        }
    }
}