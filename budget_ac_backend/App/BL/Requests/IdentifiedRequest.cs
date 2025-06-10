using System.Security.Claims;

namespace budget_ac_backend.App.BL.Requests;

public abstract class IdentifiedRequest<T> {
    public async Task<IResult> Handle(HttpContext context, T request) {
        string? sub = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(sub, out int userId)) {
            throw new ArgumentException($"Invalid user id: {sub}");
        }

        return await OnHandle(userId, request);
    }

    protected abstract Task<IResult> OnHandle(int userId, T request);
}