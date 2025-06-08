using budget_ac_backend.App.Data;
using Microsoft.EntityFrameworkCore;

namespace budget_ac_backend.App.Repository.SqlLite;

public class AppDbContext : DbContext {
    public DbSet<User> Users => Set<User>();
    public DbSet<OperationCategory> OperationCategories => Set<OperationCategory>();
    public DbSet<Operation> Operations => Set<Operation>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name).HasMaxLength(64).IsRequired();
            entity.Property(u => u.Email).HasMaxLength(64).IsRequired();
            entity.Property(u => u.Salt).IsRequired();
            entity.Property(u => u.PasswordHash).IsRequired();
            entity.Property(u => u.RegisteredAt).IsRequired();
            entity.Property(u => u.RefreshToken).HasMaxLength(32).IsRequired();
            entity.Property(u => u.RefreshExpiresAt).IsRequired();
        });

        modelBuilder.Entity<OperationCategory>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name).HasMaxLength(64).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(256);
            
            entity.HasOne(c => c.Creator)
                .WithMany()
                .HasForeignKey(c => c.CreatorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Operation>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Money).IsRequired();
            entity.Property(o => o.Date).IsRequired();
            entity.Property(o => o.Description).HasMaxLength(256);

            entity.HasOne(o => o.Owner)
                .WithMany()
                .HasForeignKey(o => o.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Category)
                .WithMany()
                .HasForeignKey(o => o.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}