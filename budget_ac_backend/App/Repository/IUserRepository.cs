using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Repository;

public interface IUserRepository {
    Task<User?> GetUserByEmail(string email);
    Task<User?> GetUserByRefreshToken(string refreshToken);

    Task<User?> CreateUser(
        string email,
        byte[] salt,
        byte[] passwordHash,
        string refreshToken,
        DateTime refreshTokenExpiresAt);
}