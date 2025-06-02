using Serilog;
using Serilog.Events;
using ILogger = Serilog.ILogger;

namespace budget_ac_backend.App.Logging;

public class FileLoggerBuilder {
    public ILogger Build(
        LogEventLevel logLevel = LogEventLevel.Information,
        string path = "Logs/log-.log",
        string format = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level}] {Message}{NewLine}{Exception}",
        int fileTimeLimitInDays = 30) {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(logLevel)
            .WriteTo.File(
                path: path,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                rollOnFileSizeLimit: false,
                retainedFileTimeLimit: TimeSpan.FromDays(fileTimeLimitInDays),
                outputTemplate: format)
            .CreateLogger();

        Log.Information("Logger initialized");
        return Log.Logger;
    }
}