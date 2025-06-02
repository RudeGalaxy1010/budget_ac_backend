using budget_ac_backend.App.Auth;
using budget_ac_backend.App.Logging;
using budget_ac_backend.App.Middleware;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using ILogger = Serilog.ILogger;

ILogger logger = new FileLoggerBuilder().Build();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", false, true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.AddAuth();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware
app.UseMiddleware<JsonExceptionHandlerMiddleWare>();

// Requests
AuthMap authMap = new AuthMap(app);
authMap.MapRoutes();

// Startup
app.Lifetime.ApplicationStarted.Register(() => {
    logger.Information("Started");
    IServer? server = app.Services.GetService<IServer>();
    IServerAddressesFeature? features = server?.Features.Get<IServerAddressesFeature>();

    if (features != null) {
        foreach (string addr in features.Addresses) {
            logger.Information("Listening: {Address}", addr);
        }
    }
});

app.Lifetime.ApplicationStopping.Register(() => {
    logger.Information("Shutting down...");
});

app.Run();