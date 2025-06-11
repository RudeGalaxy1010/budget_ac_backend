using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Repository;

public interface IOperationRepository {
    public Task<Operation?> GetOperationById(int id);
    public Task<List<Operation>> GetOperations(int userId, DateTime from, DateTime to);
    public Task<Operation> CreateOperation(int userId, int categoryId, decimal money, DateTime date, string description);
    public Task UpdateOperation(int id, int categoryId, decimal money, DateTime date, string description);
    public Task DeleteOperation(int id);
}