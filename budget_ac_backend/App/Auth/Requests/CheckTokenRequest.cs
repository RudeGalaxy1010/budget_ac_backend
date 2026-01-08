using System.Security.Claims;
using budget_ac_backend.App.Data;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Utils;


namespace budget_ac_backend.App.Auth.Requests;

public class CheckTokenRequest(IUserRepository userRepository) {
    private readonly IUserRepository _userRepository = userRepository.ThrowIfArgumentNull();

    public async Task<IResult> Handle(HttpContext context) {
        string? sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? expired = context.User.FindFirst("exp")?.Value;

        if (sub == null
            || !int.TryParse(sub, out int userId)
            || expired == null
            || !long.TryParse(expired, out long expSeconds)) {
            return Results.Unauthorized();
        }

        DateTimeOffset expirationDate = DateTimeOffset.FromUnixTimeSeconds(expSeconds);

        if (expirationDate < DateTimeOffset.UtcNow) {
            return Results.Unauthorized();
        }

        User? user = await _userRepository.GetUserById(userId);

        if (user == null) {
            return Results.Unauthorized();
        }

        return Results.Ok();
    }
}
