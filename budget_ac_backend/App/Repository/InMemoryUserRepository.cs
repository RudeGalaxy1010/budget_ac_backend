using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;

namespace budget_ac_backend.App.Repository;

public class InMemoryUserRepository : IUserRepository {
    private readonly List<User> _users;

    public InMemoryUserRepository(List<User>? users = default) {
        _users = users ?? new List<User>();
    }

    public Task<User?> CreateUser(
        string email,
        byte[] salt,
        byte[] passwordHash,
        string refreshToken,
        DateTime refreshTokenExpiresAt) {
        email.ThrowIfNullOrWhiteSpace(nameof(email));
        salt.ThrowIfArgumentNull();
        passwordHash.ThrowIfArgumentNull();

        if (_users.Any(user => user.Email == email)) {
            return Task.FromResult<User?>(null);
        }

        int id = _users.Count;

        User user = new User {
            Id = id,
            Email = email,
            Name = $"user-{id}",
            Salt = salt,
            PasswordHash = passwordHash,
            RegisteredAt = DateTime.UtcNow,
            RefreshToken = refreshToken,
            RefreshExpiresAt = refreshTokenExpiresAt
        };

        _users.Add(user);
        return Task.FromResult<User?>(user);
    }

    public Task<User?> GetUserByEmail(string email) {
        email.ThrowIfNullOrWhiteSpace(nameof(email));

        return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
    }

    public Task<User?> GetUserByRefreshToken(string refreshToken) {
        refreshToken.ThrowIfArgumentNull();
        User? user = _users.FirstOrDefault(user => user.RefreshToken == refreshToken);
        return Task.FromResult(user);
    }

}