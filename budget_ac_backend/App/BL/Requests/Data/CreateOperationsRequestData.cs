namespace budget_ac_backend.App.BL.Requests.Data;

public record CreateOperationsRequestData(string CategoryName, decimal Money, DateTime Date);
