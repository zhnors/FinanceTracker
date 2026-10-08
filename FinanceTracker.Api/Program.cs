using FinanceTracker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IFinanceService, FinanceService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


var service = app.Services.GetRequiredService<IFinanceService>();

service.AddRecord(new IncomeRecord {
    Id = 1,
    Title = "Salary",
    Amount = 30000,
    Date = DateTime.Now,
    Category = "Salary",
    Description = "Monthly salary",
    IncomeType = FinanceTracker.Enums.IncomeTypes.Salary
});

service.AddRecord(new ExpenseRecord
{
    Id = 3,
    Title = "Rent",
    Amount = 12000,
    Date = DateTime.Now,
    Category = "Rent",
    Description = "Monthly apartment rent",
    ExpenseType = FinanceTracker.Enums.ExpenseTypes.Rent
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
