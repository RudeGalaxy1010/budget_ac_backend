using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;
using Serilog;

namespace budget_ac_backend.App.Auth.Requests;

public class CreateUserRequest {
    private readonly IPasswordHashService _passwordHashService;
    private readonly ITokenGeneratorService _tokenGeneratorService;
    private readonly IValidator<CreateUserRequestData> _loginRequestValidator;
    private readonly IUserRepository _userRepository;

    public CreateUserRequest(
        IValidator<CreateUserRequestData> loginRequestValidator,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        ITokenGeneratorService tokenGeneratorService) {
        _passwordHashService = passwordHashService;
        _loginRequestValidator = loginRequestValidator.ThrowIfArgumentNull();
        _userRepository = userRepository.ThrowIfArgumentNull();
        _tokenGeneratorService = tokenGeneratorService.ThrowIfArgumentNull();
    }

    public async Task<IResult> Create(HttpContext context, CreateUserRequestData request) {
        try {
            Log.Information($"{nameof(CreateUserRequest)} from " +
                            $"{context.Connection.RemoteIpAddress}:{context.Connection.RemotePort}, params: {request}");
            ValidationResult validationResult = await _loginRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid) {
                return Results.BadRequest(new { error = ErrorMessages.InvalidData });
            }

            byte[] salt = _passwordHashService.GenerateSalt();
            byte[] passwordHash = _passwordHashService.HashPassword(request.Password, salt);
            string refreshToken = _tokenGeneratorService.GenerateRefreshToken();
            DateTime refreshTokenExpirationDate = _tokenGeneratorService.GetRefreshTokenExpirationDate();

            User? userProfile = await _userRepository.CreateUser(request.Email, salt, passwordHash, refreshToken, refreshTokenExpirationDate);

            if (userProfile == null) {
                return Results.BadRequest(new { error = ErrorMessages.UserAlreadyExists });
            }

            return Results.Ok(new {
                userId = userProfile.Id,
                accessToken = _tokenGeneratorService.GenerateAuthToken(userProfile),
                refreshToken = refreshToken
            });
        }
        catch (Exception exception) {
            Log.Error(exception, string.Empty);
            return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: ErrorMessages.UnexpectedError);
        }
    }
}