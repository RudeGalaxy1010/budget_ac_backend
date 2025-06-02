namespace budget_ac_backend.App.Data;

public class UserProfile : IUserProfile {
    public required string Id { get; set; }
    public required string Name { set; get; }
    public required string Email { get; set; }
    public byte[] PasswordHash { get; set; } = [];
    public byte[] PasswordSalt { get; set; } = [];
}