using Microsoft.EntityFrameworkCore;
using FinanceApp.API.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FinanceApp.API.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Wallet> Wallets { get; set; }

    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
    }

    public override int SaveChanges()
    {
        SetTimestamps();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        SetTimestamps();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        SetTimestamps();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void SetTimestamps()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                TrySetDateTimeProperty(entry, "CreatedAt", now);
                TrySetDateTimeProperty(entry, "UpdatedAt", now);
            }

            if (entry.State == EntityState.Modified)
            {
                TrySetDateTimeProperty(entry, "UpdatedAt", now);
            }
        }
    }

    private static void TrySetDateTimeProperty(EntityEntry entry, string propertyName, DateTime value)
    {
        var property = entry.Properties.FirstOrDefault(p => p.Metadata.Name == propertyName);

        if (property is not null && property.Metadata.ClrType == typeof(DateTime))
        {
            property.CurrentValue = value;
        }
    }
}
