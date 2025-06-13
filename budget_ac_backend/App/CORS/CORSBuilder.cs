namespace budget_ac_backend.App.CORS;

public static class CORSBuilder {
    public const string AnyOriginPolicyName = "AllowAnyOrigin";

    public static void AllowCORSAnyOrigin(this WebApplicationBuilder builder) {
        builder.Services.AddCors(options => {
            options.AddPolicy(AnyOriginPolicyName,
                policyBuilder => {
                    policyBuilder.WithOrigins("*")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });
    }
}