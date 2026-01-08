using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Auth.Requests;

public class RefreshTokenRequest(
    IValidator<RefreshTokenRequestData> validator,
    IUserRepository userRepository,
    ITokenGeneratorService tokenGeneratorService) {
    private readonly ITokenGeneratorService _tokenGeneratorService = tokenGeneratorService.ThrowIfArgumentNull();
    private readonly IUserRepository _userRepository = userRepository.ThrowIfArgumentNull();
    private readonly IValidator<RefreshTokenRequestData> _validator = validator.ThrowIfArgumentNull();

    public async Task<IResult> Handle(RefreshTokenRequestData request) {
        ValidationResult result = await _validator.ValidateAsync(request);

        if (!result.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        User? user = await _userRepository.GetUserByRefreshToken(request.RefreshToken);

        if (user == null || DateTime.UtcNow > user.RefreshExpiresAt.ToUniversalTime()) {
            return Results.BadRequest(new { error = ErrorMessages.TokenExpired });
        }

        string refreshToken = _tokenGeneratorService.GenerateRefreshToken();
        DateTime refreshTokenExpirationDate = _tokenGeneratorService.GetRefreshTokenExpirationDate();
        user.RefreshToken = refreshToken;
        user.RefreshExpiresAt = refreshTokenExpirationDate;
        await _userRepository.UpdateUser(user);

        return Results.Ok(new {
            accessToken = _tokenGeneratorService.GenerateAuthToken(user),
            refreshToken = refreshToken
        });
    }
}
