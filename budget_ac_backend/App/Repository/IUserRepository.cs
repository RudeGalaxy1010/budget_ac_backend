using budget_ac_backend.App.Auth.Data;
using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Repository;

public interface IUserRepository {

    Task<IUserProfile?> CreateUser(string email, string password);
    Task<IUserProfile?> GetUserByEmail(string email);
    Task<RefreshToken> GetRefreshToken(string refreshToken);
    Task<IUserProfile?> GetUserByRefreshToken(RefreshToken refreshToken);
    Task<IUserProfile?> GetUserById(string id);
    Task UpdateRefreshToken(string userId, RefreshToken refreshToken);
}