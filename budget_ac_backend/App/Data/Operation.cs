using System.ComponentModel.DataAnnotations;

namespace budget_ac_backend.App.Data;

public class Operation {
    [Key] public int Id { get; init; }
    public required int UserId { get; init; }
    public required int CategoryId { get; set; }
    public required decimal Money { get; set; }
    public required DateTime Date { get; set; }
    [MaxLength(256)] public required string Description { get; set; }

    public User User { get; init; } = null!;
    public Category Category { get; init; } = null!;
}