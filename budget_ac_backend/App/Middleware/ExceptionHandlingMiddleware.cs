using Serilog;

namespace budget_ac_backend.App.Middleware;

public class ExceptionHandlingMiddleware {
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context) {
        try {
            await _next(context);
        }
        catch (Exception exception) {
            Log.Error(exception, "Unhandled exception occurred");

            IResult result = Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "An error occurred while processing request.",
                detail: "Something went wrong.",
                instance: context.TraceIdentifier
            );

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            await result.ExecuteAsync(context);
        }
    }
}