using FinanceTracker.Api.Data;
using FinanceTracker.Api.Data.Entities;
using FinanceTracker.Enums;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Api.Services;

public class EfFinanceService : IFinanceService
{
    private const int DemoUserId = 1;
    private const string IncomeTypeName = nameof(RecordTypes.Income);
    private const string ExpenseTypeName = nameof(RecordTypes.Expense);

    private readonly FinanceDbContext _context;

    public event LargeExpenseDelegate? LargeExpense;

    public EfFinanceService(FinanceDbContext context)
    {
        _context = context;
    }

    public async Task AddRecordAsync(FinancialRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        FinancialRecordValidator.Validate(record);

        var typeName = record is IncomeRecord ? IncomeTypeName : ExpenseTypeName;

        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == record.Category && c.RecordType.Name == typeName);

        if (category is null)
            throw new ArgumentException($"Category '{record.Category}' does not exist for {typeName}");

        var entity = new FinancialRecordEntity
        {
            Id = record.Id,

            Title = record.Title,

            Amount = record.Amount,

            Date = record.Date,

            Description = record.Description,

            UserId = DemoUserId,

            CategoryId = category.Id,

            RecordTypeId = category.RecordTypeId
        };

        _context.FinancialRecords.Add(entity);
        await _context.SaveChangesAsync();

        record.Id = entity.Id;

        if (record is ExpenseRecord && record.Amount >= FinanceRules.LargeExpenseLimit)
        {
            LargeExpense?.Invoke(record);
        }
    }

    public Task<bool> DeleteRecordAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyList<FinancialRecord>> GetAllRecordsAsync()
    {
        var entities = await RecordsWithDetails()
         .OrderBy(r => r.Id)
         .ToListAsync();

        return entities.Select(ToModel).ToList();
    }

    public Task<IReadOnlyList<FinancialRecord>> SearchByCategoryAsync(string category)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime date)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime fromDate, DateTime toDate)
    {
        throw new NotImplementedException();
    }

    public async Task<FinancialRecord?> GetRecordByIdAsync(int id)
    {
        var entity = await RecordsWithDetails()
        .FirstOrDefaultAsync(r => r.Id == id);

        return entity is null ? null : ToModel(entity);
    }

    public Task<decimal> IncomeAmountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<decimal> ExpenseAmountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<decimal> TotalAmountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<ExpenseRecord>> GetTopExpensesAsync(int count = 3)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateRecordAsync(FinancialRecord record)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAllRecordsAsync()
    {
        throw new NotImplementedException();
    }

    private IQueryable<FinancialRecordEntity> RecordsWithDetails()
    {
        return _context.FinancialRecords
            .AsNoTracking()
            .Include(r => r.Category)
            .Include(r => r.RecordType);
    }

    private static FinancialRecord ToModel(FinancialRecordEntity entity)
    {
        FinancialRecord record = entity.RecordType.Name switch
        {
            "Income" => new IncomeRecord(),
            "Expense" => new ExpenseRecord(),
            _ => throw new InvalidOperationException($"Unknown record type: {entity.RecordType.Name}")
        };

        record.Id = entity.Id;
        record.Title = entity.Title;
        record.Amount = entity.Amount;
        record.Date = entity.Date;
        record.Category = entity.Category.Name;
        record.Description = entity.Description;

        return record;
    }

}