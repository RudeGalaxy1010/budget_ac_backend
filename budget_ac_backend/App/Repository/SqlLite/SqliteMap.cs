using budget_ac_backend.App.Data;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class SqliteMap {
    private const int TestUserId = -1;
    private readonly WebApplication _app;

    public SqliteMap(WebApplication app) {
        _app = app;
    }

    public async Task<IUserRepository> AddRepository(AppDbContext appDbContext) {
        bool hasTestUser = await appDbContext.Users.AnyAsync(u => u.Id == TestUserId);

        // Test user
        // Login: john.doe@gmail.com
        // Password: string
        if (_app.Environment.IsDevelopment() && !hasTestUser) {
            User testUser = new User {
                Id = TestUserId,
                Name = "John Doe",
                Email = "john.doe@gmail.com",
                PasswordHash = [30, 45, 115, 9, 174, 46, 251, 240, 12, 83, 14, 124, 209, 15, 118, 218],
                Salt = [221, 126, 68, 171, 79, 190, 50, 187, 252, 113, 105, 127, 203, 117, 8, 229],
                RegisteredAt = DateTime.UtcNow,
                RefreshToken = "##########",
                RefreshExpiresAt = DateTime.UtcNow
            };

            appDbContext.Users.Add(testUser);
            await appDbContext.SaveChangesAsync();
        }
        else if (hasTestUser) {
            User testUser = appDbContext.Users.First(user => user.Id == TestUserId);
            appDbContext.Users.Remove(testUser);
            await appDbContext.SaveChangesAsync();
        }

        IUserRepository userRepository = new UserRepository(appDbContext);
        return userRepository;
    }
}