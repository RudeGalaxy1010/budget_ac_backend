namespace budget_ac_backend.App.Operations.Requests.Data;

public record EditOperationRequestData(int Id, string CategoryName, decimal Money, DateTime Date);