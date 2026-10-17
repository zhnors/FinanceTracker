using FinanceTracker.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RecordType> RecordTypes { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;

}
