using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

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

    public async Task<IResult> Handle(CreateUserRequestData request) {
        ValidationResult validationResult = await _loginRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        byte[] salt = _passwordHashService.GenerateSalt();
        byte[] passwordHash = _passwordHashService.HashPassword(request.Password, salt);
        string refreshToken = _tokenGeneratorService.GenerateRefreshToken();
        DateTime refreshTokenExpirationDate = _tokenGeneratorService.GetRefreshTokenExpirationDate();

        User? user = await _userRepository.CreateUser(request.Email, salt, passwordHash, refreshToken, refreshTokenExpirationDate);

        if (user == null) {
            return Results.BadRequest(new { error = ErrorMessages.UserAlreadyExists });
        }

        return Results.Ok(new {
            userId = user.Id,
            accessToken = _tokenGeneratorService.GenerateAuthToken(user),
            refreshToken = refreshToken
        });
    }
}