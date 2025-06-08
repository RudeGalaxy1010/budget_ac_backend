namespace budget_ac_backend.App.Data;

public class Operation {
    public required int Id { get; set; }
    public int OwnerId { get; init; }
    public required User Owner { get; set; }
    public int CategoryId { get; init; }
    public required OperationCategory Category { get; set; }
    public required decimal Money { get; set; }
    public required DateTime Date { get; set; }
    public string? Description { get; set; }
}