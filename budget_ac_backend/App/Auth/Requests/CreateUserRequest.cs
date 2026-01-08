using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using FluentValidation.Results;

namespace budget_ac_backend.App.Auth.Requests;

public class CreateUserRequest(
    IValidator<CreateUserRequestData> loginRequestValidator,
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    ITokenGeneratorService tokenGeneratorService) {
    private readonly ITokenGeneratorService _tokenGeneratorService = tokenGeneratorService.ThrowIfArgumentNull();
    private readonly IValidator<CreateUserRequestData> _loginRequestValidator = loginRequestValidator.ThrowIfArgumentNull();
    private readonly IUserRepository _userRepository = userRepository.ThrowIfArgumentNull();

    public async Task<IResult> Handle(CreateUserRequestData request) {
        ValidationResult validationResult = await _loginRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        User? user = await _userRepository.GetUserByEmail(request.Email);

        if (user != null) {
            return Results.BadRequest(new { error = ErrorMessages.UserAlreadyExists });
        }

        byte[] salt = passwordHashService.GenerateSalt();
        byte[] passwordHash = passwordHashService.HashPassword(request.Password, salt);
        string refreshToken = _tokenGeneratorService.GenerateRefreshToken();
        DateTime refreshTokenExpirationDate = _tokenGeneratorService.GetRefreshTokenExpirationDate();

        user = await _userRepository.CreateUser(request.Email, salt, passwordHash, refreshToken, refreshTokenExpirationDate);

        if (user == null) {
            return Results.BadRequest(new { error = ErrorMessages.UnexpectedError });
        }

        return Results.Ok(new {
            accessToken = _tokenGeneratorService.GenerateAuthToken(user),
            refreshToken = refreshToken
        });
    }
}
