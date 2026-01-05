using System.ComponentModel.DataAnnotations;

namespace budget_ac_backend.App.Data;

public class Category {
    [Key] public long Id { get; init; }
    [MaxLength(64)] public required string Name { get; set; }
    public required long UserId { get; init; }

    public User User { get; init; } = null!;
    public ICollection<Operation> Operations { get; init; } = new List<Operation>();
}
