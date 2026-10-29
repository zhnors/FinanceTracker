using FinanceTracker.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker.Api.Tests;

public class FinanceApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string TestConnectionString =
        "Server=localhost;Database=FinanceTrackerEf_Tests;Trusted_Connection=True;TrustServerCertificate=True";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:FinanceDb", TestConnectionString);
    }

    public async ValueTask InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

        var databaseName = context.Database.GetDbConnection().Database;
        if (!databaseName.EndsWith("_Tests"))
            throw new InvalidOperationException($"Refusing to reset database '{databaseName}'");

        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }
}