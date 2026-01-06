using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;
using Microsoft.IdentityModel.Tokens;

namespace budget_ac_backend.App.Auth.Services;

public class TokenGeneratorService(IKeystoreService keystoreService) : ITokenGeneratorService {
    private const int TokenLifeTimeInMinutes = 120;
    private const int RefreshTokenLifeTimeInDays = 1;
    private const int RefreshTokenSizeInBytes = 32;

    private readonly IKeystoreService _keystoreService = keystoreService.ThrowIfArgumentNull();

    public string GenerateAuthToken(User user) {
        JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
        byte[] key = Encoding.ASCII.GetBytes(_keystoreService.GetSecretKey());

        Claim[] claims = [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Name),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        ];

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(TokenLifeTimeInMinutes),
            Issuer = AuthBuilder.IssuerName,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        SecurityToken? token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken() {
        byte[] randomNumber = new byte[RefreshTokenSizeInBytes];

        using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create()) {
            randomNumberGenerator.GetBytes(randomNumber);
        }

        return Convert.ToBase64String(randomNumber);
    }

    public DateTime GetRefreshTokenExpirationDate() {
        return DateTime.UtcNow.AddDays(RefreshTokenLifeTimeInDays);
    }
}
