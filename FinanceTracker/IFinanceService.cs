
namespace FinanceTracker
{

    public delegate void LargeExpenseDelegate(FinancialRecord record);

    public interface IFinanceService
    {
        event LargeExpenseDelegate? LargeExpense;

        Task AddRecordAsync(FinancialRecord record);

        Task<bool> DeleteRecordAsync(int id);

        Task<IReadOnlyList<FinancialRecord>> GetAllRecordsAsync();

        Task<IReadOnlyList<FinancialRecord>> SearchByCategoryAsync(string category);

        Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime date);

        Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime fromDate, DateTime toDate);

        Task<FinancialRecord?> GetRecordByIdAsync(int id);

        Task<decimal> IncomeAmountAsync();

        Task<decimal> ExpenseAmountAsync();

        Task<decimal> TotalAmountAsync();

        Task<IReadOnlyList<ExpenseRecord>> GetTopExpensesAsync(int count = 3);

        Task<bool> UpdateRecordAsync(FinancialRecord record);

        Task DeleteAllRecordsAsync();

    }
}
