using System.ComponentModel.DataAnnotations;

namespace budget_ac_backend.App.Data;

public class User {
    [Key] public int Id { get; init; }
    public required string Name { set; get; }
    public required string Email { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required DateTime RegisteredAt { get; set; }
    public required string RefreshToken { get; set; }
    public required DateTime RefreshExpiresAt { get; set; }
}