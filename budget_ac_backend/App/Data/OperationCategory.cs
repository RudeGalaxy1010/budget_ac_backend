namespace budget_ac_backend.App.Data;

public class OperationCategory {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required int CreatorId { get; init; }
    public required User Creator { get; set; }
}