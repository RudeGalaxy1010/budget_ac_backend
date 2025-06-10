using budget_ac_backend.App.Auth.Requests;
using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;

namespace budget_ac_backend.App.Auth;

public class AuthMap {
    private const string CreateUserRoute = "/auth/register";
    private const string LoginRoute = "/auth/login";
    private const string RefreshTokenRoute = "/auth/refresh";

    private readonly WebApplication _app;

    private readonly CreateUserRequest _createUserRequest;
    private readonly LoginUserRequest _loginUserRequest;
    private readonly RefreshTokenRequest _refreshTokenRequest;

    public AuthMap(WebApplication app, IUserRepository userRepository) {
        _app = app;
        _app.UseAuthentication();
        _app.UseAuthorization();

        IValidator<CreateUserRequestData> createUserDataValidator = _app.Services.GetService<IValidator<CreateUserRequestData>>().ThrowIfArgumentNull();
        IValidator<LoginUserRequestData> loginDataValidator = app.Services.GetService<IValidator<LoginUserRequestData>>().ThrowIfArgumentNull();
        IValidator<RefreshTokenRequestData> refreshTokenDataValidator = app.Services.GetService<IValidator<RefreshTokenRequestData>>().ThrowIfArgumentNull();
        ITokenGeneratorService tokenGeneratorService = _app.Services.GetService<ITokenGeneratorService>().ThrowIfArgumentNull();
        IPasswordHashService passwordHashService = _app.Services.GetService<IPasswordHashService>().ThrowIfArgumentNull();

        _createUserRequest = new CreateUserRequest(createUserDataValidator, userRepository, passwordHashService, tokenGeneratorService);
        _loginUserRequest = new LoginUserRequest(loginDataValidator, userRepository, passwordHashService, tokenGeneratorService);
        _refreshTokenRequest = new RefreshTokenRequest(refreshTokenDataValidator, userRepository, tokenGeneratorService);
    }

    public void MapRoutes() {
        _app.MapPost(CreateUserRoute, (HttpContext context, CreateUserRequestData request) =>
            _createUserRequest.Handle(context, request)).AllowAnonymous();

        _app.MapPost(LoginRoute, (HttpContext context, LoginUserRequestData request) =>
            _loginUserRequest.Handle(context, request)).AllowAnonymous();

        _app.MapPost(RefreshTokenRoute, (HttpContext context, RefreshTokenRequestData request) =>
            _refreshTokenRequest.Handle(context, request)).AllowAnonymous();
    }
}