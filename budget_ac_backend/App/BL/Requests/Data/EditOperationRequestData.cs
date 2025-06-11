namespace budget_ac_backend.App.BL.Requests.Data;

public record EditOperationRequestData(int OperationId, int CategoryId, decimal Money, DateTime Date, string Description);