using budget_ac_backend.App.Repository.SqlLite;

namespace budget_ac_backend.App.Repository;

public static class RepositoryBuilder {
    public static void AddRepository(this WebApplicationBuilder builder) {
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IOperationRepository, OperationRepository>();
    }
}
