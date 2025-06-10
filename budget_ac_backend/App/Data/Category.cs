using System.ComponentModel.DataAnnotations;

namespace budget_ac_backend.App.Data;

public class Category {
    [Key] public int Id { get; init; }
    [MaxLength(64)] public required string Name { get; set; }
    [MaxLength(256)] public required string Description { get; set; }
    public required int UserId { get; init; }

    public User User { get; set; } = null!;
    public ICollection<Operation> Operations { get; set; } = new List<Operation>();
}