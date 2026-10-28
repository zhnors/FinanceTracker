using FinanceTracker.Api.Data;
using FinanceTracker.Api.Data.Entities;
using FinanceTracker.Api.Infrastructure;
using FinanceTracker.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FinanceTracker.Api.Services;

public class EfFinanceService : IFinanceService
{
    public event LargeExpenseDelegate? LargeExpense;
    private const string IncomeTypeName = nameof(RecordTypes.Income);
    private const string ExpenseTypeName = nameof(RecordTypes.Expense);

    private readonly FinanceDbContext _context;

    private readonly ILogger<EfFinanceService> _logger;

    private readonly FinanceOptions _options;

    public EfFinanceService(
        FinanceDbContext context,
        ILogger<EfFinanceService> logger,
        IOptions<FinanceOptions> options)
    {
        _context = context;
        _logger = logger;
        _options = options.Value;
    }

    public async Task AddRecordAsync(FinancialRecord record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        FinancialRecordValidator.Validate(record);

        var typeName = record is IncomeRecord ? IncomeTypeName : ExpenseTypeName;

        var category = await FindCategoryAsync(record.Category, typeName, cancellationToken);

        var entity = new FinancialRecordEntity
        {
            Title = record.Title,

            Amount = record.Amount,

            Date = record.Date,

            Description = record.Description,

            UserId = _options.DemoUserId,

            CategoryId = category.Id,

            RecordTypeId = category.RecordTypeId
        };

        _context.FinancialRecords.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        record.Id = entity.Id;

        if (record is ExpenseRecord && record.Amount >= _options.LargeExpenseLimit)
        {
            LargeExpense?.Invoke(record);

            _logger.LogWarning("Large expense detected. Id: {RecordId} amount: {Amount}", record.Id, record.Amount);
        }

        _logger.LogInformation("Record {RecordId} created. Record Type: {RecordType}, category: {Category}, amount: {Amount}",
            record.Id, typeName, category.Name, record.Amount);

    }

    public async Task<bool> DeleteRecordAsync(int id, CancellationToken cancellationToken = default )
    {
        var isFound = false;

        var entity = await _context.FinancialRecords.FindAsync(new object [] { id }, cancellationToken);

        if (entity != null) 
        {
            _context.FinancialRecords.Remove(entity);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted record: {RecordId}", entity.Id);

            isFound = true;
        }

        return isFound;        
    }

    public async Task<IReadOnlyList<FinancialRecord>> GetAllRecordsAsync(CancellationToken cancellationToken = default)
    {
        var entities = await RecordsWithDetails()
         .OrderBy(r => r.Id)
         .ToListAsync(cancellationToken);

        return entities.Select(ToModel).ToList();
    }

    public async Task<IReadOnlyList<FinancialRecord>> SearchByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        FinancialRecordValidator.ValidateSearchCategory(category);

        var entities = await RecordsWithDetails()
            .Where(r => r.Category.Name == category)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(ToModel).ToList();
    }

    public async Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        FinancialRecordValidator.ValidateSearchDate(date);

        var start = date.Date;
        var end = start.AddDays(1);

        var entities = await RecordsWithDetails()
            .Where(r => r.Date >= start && r.Date < end)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(ToModel).ToList();
    }

    public async Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        FinancialRecordValidator.ValidateSearchRange(fromDate, toDate);

        var start = fromDate.Date;
        var end = toDate.Date.AddDays(1);

        var entities = await RecordsWithDetails()
            .Where(r => r.Date >= start && r.Date < end)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);

        return entities.Select(ToModel).ToList();
    }

    public async Task<FinancialRecord?> GetRecordByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await RecordsWithDetails()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        return entity is null ? null : ToModel(entity);
    }

    public async Task<decimal> IncomeAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FinancialRecords
        .Where(r => r.RecordType.Name == IncomeTypeName)
        .SumAsync(r => r.Amount, cancellationToken);
    }

    public async Task<decimal> ExpenseAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FinancialRecords
        .Where(r => r.RecordType.Name == ExpenseTypeName)
        .SumAsync(r => r.Amount, cancellationToken);
    }

    public async Task<decimal> TotalAmountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.FinancialRecords       
        .SumAsync(r => r.RecordType.Name == ExpenseTypeName ? -r.Amount : r.Amount, cancellationToken);
    }

    public async Task<IReadOnlyList<ExpenseRecord>> GetTopExpensesAsync(int count = 3, CancellationToken cancellationToken = default)
    {

        var entities = await RecordsWithDetails()
        .Where(r => r.RecordType.Name == ExpenseTypeName)
        .OrderByDescending(r => r.Amount)
        .Take(count)
        .ToListAsync(cancellationToken);

        return entities.Select(ToModel).Cast<ExpenseRecord>().ToList();
    }

    public async Task<bool> UpdateRecordAsync(FinancialRecord record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        FinancialRecordValidator.Validate(record);

        var entity = await _context.FinancialRecords
            .Include(r => r.RecordType)
            .FirstOrDefaultAsync(r => r.Id == record.Id, cancellationToken);

        if (entity == null) return false;

        if ((record is IncomeRecord) != (entity.RecordType.Name == IncomeTypeName)) 
            throw new ArgumentException("Record type cannot be changed");

        var category = await FindCategoryAsync(record.Category, entity.RecordType.Name, cancellationToken);

        entity.Title = record.Title;
        entity.Amount = record.Amount;
        entity.Date = record.Date;
        entity.Description = record.Description;
        entity.CategoryId = category.Id;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated record: {RecordId}", entity.Id);

        return true;   
    }

    public async Task DeleteAllRecordsAsync(CancellationToken cancellationToken = default)
    {
        var count = await _context.FinancialRecords.ExecuteDeleteAsync(cancellationToken);

        _logger.LogWarning("All {Count} records were deleted", count);

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

    private async Task<Category> FindCategoryAsync(string name, string typeName, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Name == name && c.RecordType.Name == typeName, cancellationToken);

        if (category is null)
            throw new ArgumentException($"Category '{name}' does not exist for {typeName}");

        return category;
    }


}