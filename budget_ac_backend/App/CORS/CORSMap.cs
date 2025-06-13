namespace budget_ac_backend.App.CORS;

public static class CORSMap {
    public static void UseCORSAnyOriginPolicy(this WebApplication app) {
        app.UseCors(CORSBuilder.AnyOriginPolicyName);
    }
}