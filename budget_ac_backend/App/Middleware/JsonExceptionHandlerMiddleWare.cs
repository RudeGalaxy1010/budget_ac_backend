namespace budget_ac_backend.App.Middleware;

public class JsonExceptionHandlerMiddleWare(RequestDelegate next) {

    public async Task Invoke(HttpContext context) {
        try {
            await next(context);
        }
        catch (BadHttpRequestException ex) when (ex.Message.Contains("JSON")) {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { exception = "Failed to parse json" });
        }
    }
}