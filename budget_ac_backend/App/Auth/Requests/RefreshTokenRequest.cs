using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;
using Serilog;

namespace budget_ac_backend.App.Auth.Requests;

public class RefreshTokenRequest {
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<RefreshTokenRequestData> _validator;

    public RefreshTokenRequest(
        IValidator<RefreshTokenRequestData> validator,
        IUserRepository userRepository,
        ITokenGeneratorService tokenGeneratorService) {
        _validator = validator.ThrowIfArgumentNull();
        _userRepository = userRepository.ThrowIfArgumentNull();
        _tokenGeneratorService = tokenGeneratorService.ThrowIfArgumentNull();
    }

    public async Task<IResult> Refresh(HttpContext context, RefreshTokenRequestData request) {
        try {
            Log.Information($"{nameof(RefreshTokenRequest)} from " +
                            $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}, params: {request}");

            ValidationResult result = await _validator.ValidateAsync(request);

            if (!result.IsValid) {
                return Results.BadRequest(new { error = ErrorMessages.InvalidData });
            }

            User? userProfile = await _userRepository.GetUserByRefreshToken(request.RefreshToken);

            if (userProfile == null || DateTime.UtcNow > userProfile.RefreshExpiresAt.ToUniversalTime()) {
                return Results.BadRequest(new { error = ErrorMessages.TokenExpired });
            }

            string refreshToken = _tokenGeneratorService.GenerateRefreshToken();
            DateTime refreshTokenExpirationDate = _tokenGeneratorService.GetRefreshTokenExpirationDate();
            userProfile.RefreshToken = refreshToken;
            userProfile.RefreshExpiresAt = refreshTokenExpirationDate;

            return Results.Ok(new {
                accessToken = _tokenGeneratorService.GenerateAuthToken(userProfile),
                refreshToken = refreshToken
            });
        }
        catch (Exception) {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ErrorMessages.UnexpectedError);
        }
    }
}