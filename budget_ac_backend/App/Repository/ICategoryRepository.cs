using budget_ac_backend.App.Data;

namespace budget_ac_backend.App.Repository;

public interface ICategoryRepository {
    Task<Category?> GetCategoryById(int id);
    Task<Category?> GetCategoryByName(string name);
    Task<List<Category>> GetCategories(int userId, int offset, int limit);
    Task<Category> CreateCategory(string name, string description, int userId);
    Task DeleteCategory(int id);
    Task UpdateCategory(Category category);
}