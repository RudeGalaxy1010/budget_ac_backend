using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class UserRepository : IUserRepository {
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<User?> GetUserById(int id) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetUserByEmail(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetUserByRefreshToken(string refreshToken) =>
        await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

    public async Task UpdateUser(User user) {
        user.ThrowIfArgumentNull();
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> CreateUser(string email, byte[] salt, byte[] passwordHash, string refreshToken, DateTime refreshTokenExpiresAt) {
        User user = new User {
            Email = email,
            Name = $"user-{Guid.NewGuid()}",
            Salt = salt,
            PasswordHash = passwordHash,
            RegisteredAt = DateTime.UtcNow,
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAt
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();
}
