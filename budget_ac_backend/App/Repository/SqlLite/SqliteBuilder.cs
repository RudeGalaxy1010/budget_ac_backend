using budget_ac_backend.App.Utils;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public static class SqliteBuilder {
    private const string ConnectionStringsPropertyName = "ConnectionStrings";
    private const string SqliteConnectionStringPropertyName = "Sqlite";

    public static void AddSqlite(this WebApplicationBuilder builder) {
        const string propertyPath = $"{ConnectionStringsPropertyName}:{SqliteConnectionStringPropertyName}";
        string connectionString = builder.Configuration[propertyPath].ThrowIfNullOrWhiteSpace("ConnectionString");
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
    }
}