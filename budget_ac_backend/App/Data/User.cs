using System.ComponentModel.DataAnnotations;

namespace budget_ac_backend.App.Data;

public class User {
    [Key] public long Id { get; init; }
    [MaxLength(64)] public required string Name { set; get; }
    [MaxLength(64)] public required string Email { get; set; }
    public required byte[] Salt { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required DateTime RegisteredAt { get; set; }
    [MaxLength(32)] public required string RefreshToken { get; set; }
    public required DateTime RefreshExpiresAt { get; set; }

    public ICollection<Category> Categories { get; init; } = new List<Category>();
    public ICollection<Operation> Operations { get; init; } = new List<Operation>();
}