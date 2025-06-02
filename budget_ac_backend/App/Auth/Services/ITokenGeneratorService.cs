using budget_ac_backend.App.Auth.Data;
using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Auth.Services;

public interface ITokenGeneratorService {
    string GenerateAuthToken(IUserProfile userProfile);
    RefreshToken GenerateRefreshToken();
}