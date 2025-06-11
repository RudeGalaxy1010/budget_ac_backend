using budget_ac_backend.App.Data;
using budget_ac_backend.App.Utils;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class OperationRepository : IOperationRepository {
    private readonly AppDbContext _context;

    public OperationRepository(AppDbContext context) {
        _context = context.ThrowIfArgumentNull();
    }

    public async Task<Operation?> GetOperationByUserId(int userId) {
        return await _context.Operations.FirstOrDefaultAsync(o => o.UserId == userId);
    }

    public async Task<Operation?> GetOperationById(int id) {
        return await _context.Operations.FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Operation>> GetOperations(int userId, DateTime from, DateTime to) {
        return await _context.Operations
            .Where(o => o.UserId == userId && o.Date >= from && o.Date <= to)
            .ToListAsync();
    }

    public async Task<Operation> CreateOperation(int userId, int categoryId, decimal money, DateTime date, string description) {
        User? user = _context.Users
            .Include(u => u.Categories)
            .Include(u => u.Operations)
            .FirstOrDefault(u => u.Id == userId);

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        Category? category = _context.Categories
            .Include(c => c.Operations)
            .FirstOrDefault(category => category.Id == categoryId);

        if (category == null) {
            throw new ArgumentException(ErrorMessages.CategoryNotFound);
        }

        Operation operation = new Operation {
            UserId = userId,
            CategoryId = categoryId,
            Money = money,
            Date = date,
            Description = description
        };

        user.Operations.Add(operation);
        category.Operations.Add(operation);
        await _context.Operations.AddAsync(operation);
        await _context.SaveChangesAsync();
        return operation;
    }

    public async Task UpdateOperation(int id, int categoryId, decimal money, DateTime date, string description) {
        Operation? operation = await _context.Operations.FirstOrDefaultAsync(o => o.Id == id);

        if (operation == null) {
            throw new ArgumentException(ErrorMessages.OperationNotFound);
        }

        User? user = _context.Users
            .Include(u => u.Categories)
            .Include(u => u.Operations)
            .FirstOrDefault(u => u.Id == operation.UserId);

        if (user == null) {
            throw new ArgumentException(ErrorMessages.UserNotFound);
        }

        Category? category = _context.Categories
            .Include(c => c.Operations)
            .FirstOrDefault(c => c.Id == categoryId);

        if (category == null) {
            throw new ArgumentException(ErrorMessages.CategoryNotFound);
        }

        operation.CategoryId = categoryId;
        operation.Money = money;
        operation.Date = date;
        operation.Description = description;
        _context.Operations.Update(operation);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteOperation(int id) {
        Operation? operation = await _context.Operations.FirstOrDefaultAsync(o => o.Id == id);

        if (operation == null) {
            throw new ArgumentException(ErrorMessages.OperationNotFound);
        }

        User? user = await _context.Users
            .Include(u => u.Operations)
            .FirstOrDefaultAsync(u => u.Operations.Any(c => c.Id == id));

        if (user == null) {
            throw new ArgumentException(ErrorMessages.OperationNotFound);
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
}