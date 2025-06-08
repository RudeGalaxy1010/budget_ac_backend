using budget_ac_backend.App.Data;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class UserRepository : IUserRepository {
    private readonly AppDbContext _appDbContext;

    public UserRepository(AppDbContext appDbContext) {
        _appDbContext = appDbContext;
    }

    public async Task<User?> GetUserById(int id) {
        return await _appDbContext.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User?> GetUserByEmail(string email) {
        return await _appDbContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetUserByRefreshToken(string refreshToken) {
        return await _appDbContext.Users.FirstOrDefaultAsync(user => user.RefreshToken == refreshToken);
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

        _appDbContext.Users.Add(user);
        await _appDbContext.SaveChangesAsync();
        return user;
    }

    public async Task SaveChangesAsync() {
        await _appDbContext.SaveChangesAsync();
    }
}