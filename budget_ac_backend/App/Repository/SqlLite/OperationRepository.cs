using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class OperationRepository : IOperationRepository {
    private readonly AppDbContext _context;

    public OperationRepository(AppDbContext context) {
        _context = context.ThrowIfArgumentNull();
    }

    public async Task<Operation> CreateOperation(int userId, string categoryName, decimal money, DateTime date) {
        User? user = _context.Users
            .Include(u => u.Categories)
            .Include(u => u.Operations)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        Category category = await CreateOrAddCategory(user, categoryName);

        Operation operation = new Operation {
            UserId = userId,
            CategoryId = category.Id,
            Money = money,
            Date = date,
            User = user,
            Category = category,
        };

        user.Operations.Add(operation);
        category.Operations.Add(operation);
        await _context.Operations.AddAsync(operation);
        await _context.SaveChangesAsync();
        return operation;
    }

    private async Task<Category> CreateOrAddCategory(User user, string categoryName) {
        Category? category = _context.Categories
            .Include(c => c.Operations)
            .FirstOrDefault(category => category.Name == categoryName);

        if (category != null) {
            return category;
        }

        category = new Category {
            UserId = user.Id,
            Name = categoryName,
            User = user,
        };

        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<List<Operation>> GetOperations(int userId, DateTime from, DateTime to) {
        return await _context.Operations
            .Where(o => o.UserId == userId && o.Date >= from && o.Date < to)
            .Include(o => o.Category)
            .Include(o => o.User)
            .Include(o => o.Category.User)
            .ToListAsync();
    }

    public async Task<Operation?> GetOperation(int id) {
        return await _context.Operations
            .Include(o => o.Category)
            .Include(o => o.User)
            .Include(o => o.Category.User)
            .FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task UpdateOperation(Operation operation, string categoryName, decimal money, DateTime date) {
        operation.ThrowIfArgumentNull();
        categoryName.ThrowIfArgumentNull();
        money.ThrowIfArgumentNull();
        date.ThrowIfArgumentNull();

        Category category = await CreateOrAddCategory(operation.User, categoryName);

        operation.CategoryId = category.Id;
        operation.Money = money;
        operation.Date = date;
        _context.Operations.Update(operation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOperation(Operation operation) {
        operation.ThrowIfArgumentNull();

        User? user = await _context.Users
            .Include(u => u.Operations)
            .FirstOrDefaultAsync(u => u.Operations.Any(o => o == operation));

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        Category? category = _context.Categories
            .Include(c => c.Operations)
            .FirstOrDefault(c => c.Id == operation.CategoryId);

        if (category == null) {
            throw new ArgumentException(ErrorMessages.OperationNotFound);
        }

        user.Operations.Remove(operation);
        category.Operations.Remove(operation);
        _context.Operations.Remove(operation);
        await _context.SaveChangesAsync();
    }

    public async Task<Dictionary<int, PeriodStatistics>> GetPeriodStatistics(int userId, int year) {
        User? user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        const int monthCount = 12;
        Dictionary<int, PeriodStatistics> statistics = new Dictionary<int, PeriodStatistics>(monthCount);

        for (int i = 0; i < monthCount; i++) {
            statistics.Add(i, new PeriodStatistics());
        }

        DateTime from = new DateTime(year, 1, 1);
        DateTime to = from.AddYears(1);

        await _context.Operations
            .Where(operation => operation.UserId == user.Id && operation.Date >= from && operation.Date < to)
            .ForEachAsync(operation => {
                if (operation.Money > 0) {
                    statistics[operation.Date.Month - 1].Income += operation.Money;
                }
                else {
                    statistics[operation.Date.Month - 1].Outcome += operation.Money;
                }
            });

        return statistics;
    }

    public async Task<List<TopCategorySummary>> GetTopCategories(int userId, int year, int count) {
        User? user = await _context.Users.FirstOrDefaultAsync(user => user.Id == userId);

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        DateTime from = new DateTime(year, 1, 1);
        DateTime to = from.AddYears(1);

        return _context.Operations
            .Where(o => o.UserId == userId && o.Date >= from && o.Date < to)
            .GroupBy(o => o.Category.Name)
            .Select(g => new {
                Name = g.Key,
                Count = g.Count(o => o.Money < 0),
                Outcomes = Math.Abs(g.Where(o => o.Money < 0).Sum(o => (double)o.Money))
            })
            .OrderByDescending(x => x.Count)
            .ThenByDescending(x => x.Outcomes)
            .Take(count)
            .Where(x => x.Count > 0)
            .Select(x => new TopCategorySummary {
                Name = x.Name,
                OperationCount = x.Count,
                Outcome = (decimal)x.Outcomes
            })
            .ToList();
    }
}
