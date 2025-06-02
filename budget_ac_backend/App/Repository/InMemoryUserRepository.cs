using budget_ac_backend.App.Auth.Data;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;

namespace budget_ac_backend.App.Repository;

public class InMemoryUserRepository : IUserRepository {
    private readonly List<IUserProfile> _users;
    private readonly Dictionary<string, RefreshToken> _refreshTokens;

    public InMemoryUserRepository(List<IUserProfile>? users = default, Dictionary<string, RefreshToken>? refreshTokens = default) {
        _users = users ?? new List<IUserProfile>();
        _refreshTokens = refreshTokens ?? new Dictionary<string, RefreshToken>();
    }

    public Task<IUserProfile?> CreateUser(string email, string password) {
        email.ThrowIfNullOrWhiteSpace(nameof(email));
        password.ThrowIfNullOrWhiteSpace(nameof(password));

        if (_users.Any(user => user.Email == email)) {
            return Task.FromResult<IUserProfile?>(null);
        }

        UserProfile user = new UserProfile {
            Id = Guid.NewGuid().ToString(),
            Email = email,
            Name = Guid.NewGuid().ToString()
        };

        _users.Add(user);
        return Task.FromResult<IUserProfile?>(user);
    }

    public Task<IUserProfile?> GetUserByEmail(string email) {
        email.ThrowIfNullOrWhiteSpace(nameof(email));

        return Task.FromResult(_users.FirstOrDefault(user => user.Email == email));
    }

    public Task<RefreshToken> GetRefreshToken(string refreshToken) {
        refreshToken.ThrowIfNullOrWhiteSpace(nameof(refreshToken));
        KeyValuePair<string, RefreshToken> record = _refreshTokens.FirstOrDefault(pair => pair.Value.Token == refreshToken);
        return Task.FromResult(record.Value);
    }

    public Task<IUserProfile?> GetUserByRefreshToken(RefreshToken refreshToken) {
        refreshToken.ThrowIfArgumentNull();
        KeyValuePair<string, RefreshToken> record = _refreshTokens.FirstOrDefault(pair => pair.Value == refreshToken);
        string userId = record.Key;

        return string.IsNullOrEmpty(userId)
            ? Task.FromResult<IUserProfile?>(null)
            : GetUserById(userId);
    }

    public Task<IUserProfile?> GetUserById(string id) {
        id.ThrowIfNullOrWhiteSpace(nameof(id));
        return Task.FromResult(_users.FirstOrDefault(user => user.Id == id));
    }

    public Task UpdateRefreshToken(string userId, RefreshToken refreshToken) {
        userId.ThrowIfNullOrWhiteSpace(nameof(userId));
        refreshToken.ThrowIfArgumentNull();

        _refreshTokens[userId] = refreshToken;
        return Task.CompletedTask;
    }

}