namespace budget_ac_backend.App.Data;

public interface IUserProfile {
    string Id { get; set; }
    string Name { get; set; }
    string Email { get; set; }
    byte[] PasswordHash { get; set; }
    byte[] PasswordSalt { get; set; }
}