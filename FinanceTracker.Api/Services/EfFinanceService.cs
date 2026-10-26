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

        var category = await FindCategoryAsync(record.Category, typeName);

        var entity = new FinancialRecordEntity
        {
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

    public async Task<bool> DeleteRecordAsync(int id)
    {
        var isFound = false;

        var entity = await _context.FinancialRecords.FindAsync(id);

        if (entity != null) 
        {
            _context.FinancialRecords.Remove(entity);

            await _context.SaveChangesAsync();

            isFound = true;
        }

        return isFound;        
    }

    public async Task<IReadOnlyList<FinancialRecord>> GetAllRecordsAsync()
    {
        var entities = await RecordsWithDetails()
         .OrderBy(r => r.Id)
         .ToListAsync();

        return entities.Select(ToModel).ToList();
    }

    public async Task<IReadOnlyList<FinancialRecord>> SearchByCategoryAsync(string category)
    {
        FinancialRecordValidator.ValidateSearchCategory(category);

        var entities = await RecordsWithDetails()
            .Where(r => r.Category.Name == category)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return entities.Select(ToModel).ToList();
    }

    public async Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime date)
    {
        FinancialRecordValidator.ValidateSearchDate(date);

        var start = date.Date;
        var end = start.AddDays(1);

        var entities = await RecordsWithDetails()
            .Where(r => r.Date >= start && r.Date < end)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return entities.Select(ToModel).ToList();
    }

    public async Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime fromDate, DateTime toDate)
    {
        FinancialRecordValidator.ValidateSearchRange(fromDate, toDate);

        var start = fromDate.Date;
        var end = toDate.Date.AddDays(1);

        var entities = await RecordsWithDetails()
            .Where(r => r.Date >= start && r.Date < end)
            .OrderBy(r => r.Id)
            .ToListAsync();

        return entities.Select(ToModel).ToList();
    }

    public async Task<FinancialRecord?> GetRecordByIdAsync(int id)
    {
        var entity = await RecordsWithDetails()
            .FirstOrDefaultAsync(r => r.Id == id);

        return entity is null ? null : ToModel(entity);
    }

    public async Task<decimal> IncomeAmountAsync()
    {
        return await _context.FinancialRecords
        .Where(r => r.RecordType.Name == IncomeTypeName)
        .SumAsync(r => r.Amount);
    }

    public async Task<decimal> ExpenseAmountAsync()
    {
        return await _context.FinancialRecords
        .Where(r => r.RecordType.Name == ExpenseTypeName)
        .SumAsync(r => r.Amount);
    }

    public async Task<decimal> TotalAmountAsync()
    {
        return await _context.FinancialRecords       
        .SumAsync(r => r.RecordType.Name == ExpenseTypeName ? -r.Amount : r.Amount);
    }

    public async Task<IReadOnlyList<ExpenseRecord>> GetTopExpensesAsync(int count = 3)
    {

        var entities = await RecordsWithDetails()
        .Where(r => r.RecordType.Name == ExpenseTypeName)
        .OrderByDescending(r => r.Amount)
        .Take(count)
        .ToListAsync();

        return entities.Select(ToModel).Cast<ExpenseRecord>().ToList();
    }

    public async Task<bool> UpdateRecordAsync(FinancialRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        FinancialRecordValidator.Validate(record);

        var entity = await _context.FinancialRecords
            .Include(r => r.RecordType)
            .FirstOrDefaultAsync(r => r.Id == record.Id);

        if (entity == null) return false;

        if ((record is IncomeRecord) != (entity.RecordType.Name == IncomeTypeName)) 
            throw new ArgumentException("Record type cannot be changed");

        var category = await FindCategoryAsync(record.Category, entity.RecordType.Name);

        entity.Title = record.Title;
        entity.Amount = record.Amount;
        entity.Date = record.Date;
        entity.Description = record.Description;
        entity.CategoryId = category.Id;

        await _context.SaveChangesAsync();

        return true;   
    }

    public async Task DeleteAllRecordsAsync()
    {
        await _context.FinancialRecords.ExecuteDeleteAsync();
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
            IncomeTypeName => new IncomeRecord(),
            ExpenseTypeName => new ExpenseRecord(),
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

    private async Task<Category> FindCategoryAsync(string name, string typeName)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name && c.RecordType.Name == typeName);

        if (category is null)
            throw new ArgumentException($"Category '{name}' does not exist for {typeName}");

        return category;
    }


}