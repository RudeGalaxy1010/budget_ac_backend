using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using Serilog;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace budget_ac_backend.App.Auth.Requests;

public class LoginUserRequest {
    private readonly IPasswordHashService _passwordHashService;
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly IValidator<LoginUserRequestData> _loginRequestValidator;
    private readonly IUserRepository _userRepository;

    public LoginUserRequest(
        IValidator<LoginUserRequestData> loginRequestValidator,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        ITokenGeneratorService tokenGeneratorService) {
        _passwordHashService = passwordHashService;
        _loginRequestValidator = loginRequestValidator.ThrowIfArgumentNull();
        _userRepository = userRepository.ThrowIfArgumentNull();
        _tokenGeneratorService = tokenGeneratorService.ThrowIfArgumentNull();
    }

    public async Task<IResult> Login(HttpContext context, LoginUserRequestData request) {
        try {
            Log.Information($"{nameof(LoginUserRequest)} from " +
                            $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}, params: {request}");
            ValidationResult validationResult = await _loginRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid) {
                return Results.BadRequest(new { error = ErrorMessages.InvalidData });
            }

            User? userProfile = await _userRepository.GetUserByEmail(request.Email);

            if (userProfile == null) {
                return Results.BadRequest(new { error = ErrorMessages.WrongEmailOrPassword });
            }

            if (!_passwordHashService.VerifyPassword(request.Password, userProfile.Salt, userProfile.PasswordHash)) {
                return Results.BadRequest(new { error = ErrorMessages.WrongEmailOrPassword });
            }

            userProfile.RefreshToken = _tokenGeneratorService.GenerateRefreshToken();
            await _userRepository.SaveChangesAsync();

            return Results.Ok(new {
                userId = userProfile.Id,
                accessToken = _tokenGeneratorService.GenerateAuthToken(userProfile),
                refreshToken = userProfile.RefreshToken
            });
        }
        catch (Exception) {
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ErrorMessages.UnexpectedError);
        }
    }
}