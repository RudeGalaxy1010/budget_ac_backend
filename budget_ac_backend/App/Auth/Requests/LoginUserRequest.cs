using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace budget_ac_backend.App.Auth.Requests;

public class LoginUserRequest(
    IValidator<LoginUserRequestData> loginRequestValidator,
    IUserRepository userRepository,
    IPasswordHashService passwordHashService,
    ITokenGeneratorService tokenGeneratorService) {
    private readonly ITokenGeneratorService _tokenGeneratorService = tokenGeneratorService.ThrowIfArgumentNull();
    private readonly IValidator<LoginUserRequestData> _loginRequestValidator = loginRequestValidator.ThrowIfArgumentNull();
    private readonly IUserRepository _userRepository = userRepository.ThrowIfArgumentNull();

    public async Task<IResult> Handle(LoginUserRequestData request) {
        ValidationResult validationResult = await _loginRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid) {
            return Results.BadRequest(new { error = ErrorMessages.InvalidData });
        }

        User? user = await _userRepository.GetUserByEmail(request.Email);

        if (user == null) {
            return Results.BadRequest(new { error = ErrorMessages.WrongEmailOrPassword });
        }

        if (!passwordHashService.VerifyPassword(request.Password, user.Salt, user.PasswordHash)) {
            return Results.BadRequest(new { error = ErrorMessages.WrongEmailOrPassword });
        }

        user.RefreshToken = _tokenGeneratorService.GenerateRefreshToken();
        await _userRepository.UpdateUser(user);

        return Results.Ok(new {
            accessToken = _tokenGeneratorService.GenerateAuthToken(user),
            refreshToken = user.RefreshToken
        });
    }
}
