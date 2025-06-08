using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Auth.Services;

public interface ITokenGeneratorService {
    string GenerateAuthToken(User user);
    string GenerateRefreshToken();
    DateTime GetRefreshTokenExpirationDate();
}