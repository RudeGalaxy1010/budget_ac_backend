using budget_ac_backend.App.Auth;
using budget_ac_backend.App.Logging;
using budget_ac_backend.App.Middleware;
using budget_ac_backend.App.Operations;
using budget_ac_backend.App.Repository;
using budget_ac_backend.App.Repository.SqlLite;
using budget_ac_backend.App.Statistics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

const string frontendCorsPolicyName = "AllowFrontend";
const string frontendOrigin = "http://localhost:3000";

ILogger logger = new FileLoggerBuilder().Build();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("secrets.json", false, true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options => {
    options.AddPolicy(frontendCorsPolicyName, policy => {
        policy
            .WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Sqlite")));
builder.AddRepository();
builder.AddAuth();
builder.AddOperations();
builder.AddStatistics();

WebApplication app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<JsonExceptionHandlerMiddleWare>();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(frontendCorsPolicyName);
app.UseAuthentication();
app.UseAuthorization();

// Repository
using (IServiceScope scope = app.Services.CreateScope()) {
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // TODO: Migrate
    db.Database.EnsureCreated();
}

// Requests
new AuthMap(app).MapRoutes();
new OperationsMap(app).MapRoutes();
new StatisticsMap(app).MapRoutes();

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

app.Lifetime.ApplicationStopping.Register(() => { logger.Information("Shutting down..."); });
app.Run();
