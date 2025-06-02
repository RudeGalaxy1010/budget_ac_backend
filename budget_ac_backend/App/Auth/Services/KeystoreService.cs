using budget_ac_backend.App.Utils;

namespace budget_ac_backend.App.Auth.Services;

public class KeystoreService : IKeystoreService {
    private const string KeyPropertyName = "secretKey";

    private readonly ConfigurationManager _configurationManager;

    public KeystoreService(ConfigurationManager configurationManager) {
        _configurationManager = configurationManager.ThrowIfArgumentNull();
    }

    public string GetSecretKey() {
        return _configurationManager[KeyPropertyName].ThrowIfNullOrWhiteSpace(nameof(KeyPropertyName));
    }
}