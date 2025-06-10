using Serilog;

namespace budget_ac_backend.App.Middleware;

public class RequestResponseLoggingMiddleware {
    private readonly RequestDelegate _next;

    public RequestResponseLoggingMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        // Логируем запрос
        context.Request.EnableBuffering();

        string requestBody = await ReadStreamAsync(context.Request.Body);
        context.Request.Body.Position = 0;

        Log.Information("HTTP Request from {Ip}:{Port}\n\tMethod: {Method}\n\tPath: {Path}\n\tHeaders: {Headers}\n\tBody: {Body}",
            context.Connection.RemoteIpAddress,
            context.Connection.RemotePort,
            context.Request.Method,
            context.Request.Path,
            context.Request.Headers,
            requestBody.Replace('\n', ' '));

        Stream originalBodyStream = context.Response.Body;
        await using MemoryStream responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        Log.Information("HTTP Response to {Ip}:{Port}\n\tStatusCode: {StatusCode}\n\tHeaders: {Headers}\n\tBody: {Body}",
            context.Connection.RemoteIpAddress,
            context.Connection.RemotePort,
            context.Response.StatusCode,
            context.Response.Headers,
            responseText.Replace('\n', ' '));

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private async Task<string> ReadStreamAsync(Stream stream) {
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}