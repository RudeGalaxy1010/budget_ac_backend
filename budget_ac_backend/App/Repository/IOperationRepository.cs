using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Repository;

public interface IOperationRepository {
    public Task<Operation> CreateOperation(int userId, string categoryName, decimal money, DateTime date);
    public Task<List<Operation>> GetOperations(int userId, DateTime from, DateTime to);
    public Task<Operation?> GetOperation(int id);
    public Task UpdateOperation(Operation operation, string categoryName, decimal money, DateTime date);
    public Task DeleteOperation(Operation operation);
    public Task<Dictionary<int, PeriodStatistics>> GetPeriodStatistics(int userId, int year);
    public Task<List<TopCategorySummary>> GetTopCategories(int userId, int year, int count);
}
