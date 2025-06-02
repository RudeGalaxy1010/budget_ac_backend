namespace budget_ac_backend.App.Auth.Services;

public interface IPasswordHashService {
    byte[] GenerateSalt();
    byte[] HashPassword(string password, byte[] salt);
    bool VerifyPassword(string password, byte[] salt, byte[] expectedHash);
}