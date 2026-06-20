using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
            => await _context.Users.FindAsync(userId);

        public async Task<User?> GetUserByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<User?> GetUserByRefreshTokenAsync(string token)
            => await _context.Users.FirstOrDefaultAsync(u =>
                u.RefreshToken == token && u.RefreshTokenExpiryTime > DateTime.UtcNow);

        public async Task<IEnumerable<User>> GetAllUsersAsync()
            => await _context.Users.AsNoTracking().ToListAsync();

        public async Task<bool> IsEmailTakenAsync(string email)
            => await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveRefreshTokenAsync(int userId, string? refreshToken, DateTime? expiryDate)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.SetRefreshToken(refreshToken, expiryDate);
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}