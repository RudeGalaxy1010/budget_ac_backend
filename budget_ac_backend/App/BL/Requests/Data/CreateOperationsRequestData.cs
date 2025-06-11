namespace budget_ac_backend.App.BL.Requests.Data;

public record CreateOperationsRequestData(int CategoryId, decimal Money, string Description);