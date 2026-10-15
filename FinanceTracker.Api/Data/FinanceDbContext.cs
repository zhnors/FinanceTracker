using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Data;

public class FinanceDbContext : DbContext
{
    public FinanceDbContext(DbContextOptions<FinanceDbContext> options)
        : base(options)
    {
    }
}