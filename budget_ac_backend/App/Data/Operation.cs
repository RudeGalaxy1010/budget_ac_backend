using System.ComponentModel.DataAnnotations;

namespace budget_ac_backend.App.Data;

public class Operation {
    [Key] public long Id { get; init; }
    public required long UserId { get; init; }
    public required long CategoryId { get; set; }
    public required decimal Money { get; set; }
    public required DateTime Date { get; set; }

    public User User { get; init; } = null!;
    public Category Category { get; init; } = null!;
}
