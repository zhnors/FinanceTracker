using FinanceTracker.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Data;

public class FinanceDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RecordType> RecordTypes { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<FinancialRecordEntity> FinancialRecords { get; set; } = null!;

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

            entity.HasData(new User { Id = 1, FirstName = "Demo", LastName = "User"});
        });

        modelBuilder.Entity<RecordType>(entity =>
        {
            entity.Property(rt => rt.Name).HasMaxLength(10);

            entity.HasIndex(rt => rt.Name).IsUnique();

            entity.HasData(
                new RecordType { Id = 1, Name = "Income"},
                new RecordType { Id = 2, Name = "Expense"});
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.Property(c => c.Name).HasMaxLength(100);

            entity.HasIndex(c => new { c.Name, c.RecordTypeId }).IsUnique();

            entity.HasData(
                new Category { Id = 1, Name = "Salary", RecordTypeId = 1},
                new Category { Id = 2, Name = "Gift", RecordTypeId = 1},
                new Category { Id = 3, Name = "Freelance", RecordTypeId = 1},
                new Category { Id = 4, Name = "Bonus", RecordTypeId = 1},
                new Category { Id = 5, Name = "Food", RecordTypeId = 2},
                new Category { Id = 6, Name = "Transport", RecordTypeId = 2},
                new Category { Id = 7, Name = "Rent", RecordTypeId = 2},
                new Category { Id = 8, Name = "Health", RecordTypeId = 2},
                new Category { Id = 9, Name = "Entertainment", RecordTypeId = 2},
                new Category { Id = 10, Name = "Education", RecordTypeId = 2});
        });

        modelBuilder.Entity<FinancialRecordEntity>(entity =>
        {
            entity.Property(fr => fr.Amount).HasPrecision(18, 2);

            entity.Property(fr => fr.Title).HasMaxLength(100);

            entity.Property(fr => fr.Description).HasMaxLength(150);

            entity.ToTable(t => t.HasCheckConstraint("CK_FinancialRecords_Amount", "[Amount] > 0"));

            entity.HasOne(r => r.Category)
                .WithMany()
                .HasForeignKey(r => new {r.CategoryId, r.RecordTypeId})
                .HasPrincipalKey(c => new { c.Id, c.RecordTypeId})
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.RecordType)
                .WithMany()
                .HasForeignKey(r => r.RecordTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

    }

}
