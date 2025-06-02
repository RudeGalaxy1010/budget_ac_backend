using System.Security.Cryptography;
using budget_ac_backend.App.Utils;

namespace budget_ac_backend.App.Auth.Services;

public class PasswordHashService : IPasswordHashService {
    private const int SaltSize = 16;
    private const int Iterations = 100_000;

    public byte[] GenerateSalt() {
        byte[] salt = new byte[SaltSize];
        using RandomNumberGenerator randomGenerator = RandomNumberGenerator.Create();
        randomGenerator.GetBytes(salt);
        return salt;
    }

    public byte[] HashPassword(string password, byte[] salt) {
        password.ThrowIfNullOrWhiteSpace(nameof(password));
        salt.ThrowIfArgumentNull();

        using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        return pbkdf2.GetBytes(SaltSize);
    }

    public bool VerifyPassword(string password, byte[] salt, byte[] expectedHash) {
        password.ThrowIfNullOrWhiteSpace(nameof(password));
        salt.ThrowIfArgumentNull();
        expectedHash.ThrowIfArgumentNull();

        byte[] hash = HashPassword(password, salt);
        return hash.SequenceEqual(expectedHash);
    }

}