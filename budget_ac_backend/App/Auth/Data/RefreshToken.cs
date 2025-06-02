using budget_ac_backend.App.Utils;

namespace budget_ac_backend.App.Auth.Data;

public class RefreshToken {
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }

    public RefreshToken(string token, DateTime expiresAt) {
        Token = token.ThrowIfNullOrWhiteSpace(nameof(token));
        ExpiresAt = expiresAt.ThrowIfArgumentNull();
    }
}