using budget_ac_backend.App.Utils;

namespace budget_ac_backend.App.Auth.Services;

public class KeystoreService(IConfiguration configuration) : IKeystoreService {
    private const string KeyPropertyName = "SecretKey";

    private readonly IConfiguration _configurationManager = configuration.ThrowIfArgumentNull();

    public string GetSecretKey() =>
        _configurationManager[KeyPropertyName].ThrowIfNullOrWhiteSpace(nameof(KeyPropertyName));
}
