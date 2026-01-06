using budget_ac_backend.App.Auth.Requests;
using budget_ac_backend.App.Auth.Requests.Data;
using budget_ac_backend.App.Auth.Services;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;
using FluentValidation;

namespace budget_ac_backend.App.Auth;

public class AuthMap(WebApplication app) {
    private const string CreateUserRoute = "/auth/register";
    private const string LoginRoute = "/auth/login";
    private const string RefreshTokenRoute = "/auth/refresh";
    private const string CheckTokenRoute = "/auth/check";

    public void MapRoutes() {
        app.MapPost(CreateUserRoute, CreateUser).AllowAnonymous();
        app.MapPost(LoginRoute, LoginUser).AllowAnonymous();
        app.MapPost(RefreshTokenRoute, RefreshToken).AllowAnonymous();
        app.MapGet(CheckTokenRoute, CheckToken).AllowAnonymous();
    }

    private async static Task<IResult> CreateUser(
        CreateUserRequestData request,
        IValidator<CreateUserRequestData> createUserDataValidator,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        ITokenGeneratorService tokenGeneratorService) {
        CreateUserRequest createUserRequest = new CreateUserRequest(createUserDataValidator,
            userRepository,
            passwordHashService,
            tokenGeneratorService);
        return await createUserRequest.Handle(request);
    }

    private async static Task<IResult> LoginUser(
        LoginUserRequestData request,
        IValidator<LoginUserRequestData> loginDataValidator,
        IUserRepository userRepository,
        IPasswordHashService passwordHashService,
        ITokenGeneratorService tokenGeneratorService
    ) {
        LoginUserRequest loginUserRequest = new LoginUserRequest(
            loginDataValidator,
            userRepository,
            passwordHashService,
            tokenGeneratorService);
        return await loginUserRequest.Handle(request);
    }

    private async static Task<IResult> RefreshToken(
        RefreshTokenRequestData request,
        IValidator<RefreshTokenRequestData> refreshTokenDataValidator,
        IUserRepository userRepository,
        ITokenGeneratorService tokenGeneratorService
    ) {
        RefreshTokenRequest refreshTokenRequest = new RefreshTokenRequest(
            refreshTokenDataValidator,
            userRepository,
            tokenGeneratorService);
        return await refreshTokenRequest.Handle(request);
    }

    private async static Task<IResult> CheckToken(HttpContext context, IUserRepository userRepository) {
        CheckTokenRequest checkTokenRequest = new CheckTokenRequest(userRepository);
        return await checkTokenRequest.Handle(context);
    }
}
