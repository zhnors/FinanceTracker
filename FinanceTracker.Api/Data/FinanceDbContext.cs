using FinanceTracker.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Data;

public class FinanceDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RecordType> RecordTypes { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
    : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.FirstName).HasMaxLength(50);
            entity.Property(u => u.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<RecordType>(entity =>
        { 
            entity.Property(rt => rt.Name).HasMaxLength(10);
            entity.HasIndex(rt => rt.Name).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100);
            entity.HasIndex(c => new { c.Name, c.RecordTypeId }).IsUnique();
        });
    }

}
