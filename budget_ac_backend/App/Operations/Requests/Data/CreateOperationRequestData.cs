namespace budget_ac_backend.App.Operations.Requests.Data;

public record CreateOperationRequestData(string CategoryName, decimal Money, DateTime Date);
