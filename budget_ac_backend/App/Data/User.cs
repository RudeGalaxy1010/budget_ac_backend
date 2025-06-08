namespace budget_ac_backend.App.Data;

public class User {
    public required int Id { get; set; }
    public required string Name { set; get; }
    public required string Email { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required DateTime RegisteredAt { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshExpiresAt { get; set; }
}