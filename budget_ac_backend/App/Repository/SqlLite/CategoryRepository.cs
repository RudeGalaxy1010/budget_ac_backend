using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class CategoryRepository {
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) {
        _context = context;
    }

    public async Task<Category?> GetCategoryById(int id) {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetCategoryByName(string name) {
        return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<List<Category>> GetCategories(int userId, int offset, int limit) {
        return await _context.Categories.Where(c => c.UserId == userId).Skip(limit).Take(limit).ToListAsync();
    }

    public async Task<Category> CreateCategory(string name, int userId) {
        name.ThrowIfNullOrWhiteSpace(nameof(name));
        userId.ThrowIfArgumentNull();

        User? user = await _context.Users
            .Include(u => u.Categories)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        Category category = new Category {
            Name = name,
            UserId = userId,
            User = user
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategory(int id) {
        Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category == null) {
            throw new ArgumentException(ErrorMessages.CategoryNotFound);
        }

        User? user = await _context.Users
            .Include(u => u.Categories)
            .FirstOrDefaultAsync(u => u.Categories.Any(c => c.Id == id));

        user?.Categories.Remove(category);
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategory(Category category) {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }
}