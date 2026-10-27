namespace FinanceTracker
{

    public delegate void LargeExpenseDelegate(FinancialRecord record);

    public interface IFinanceService
    {
        event LargeExpenseDelegate? LargeExpense;

        Task AddRecordAsync(FinancialRecord record, CancellationToken cancellationToken = default);

        Task<bool> DeleteRecordAsync(int id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FinancialRecord>> GetAllRecordsAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FinancialRecord>> SearchByCategoryAsync(string category, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime date, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);

        Task<FinancialRecord?> GetRecordByIdAsync(int id, CancellationToken cancellationToken = default);

        Task<decimal> IncomeAmountAsync(CancellationToken cancellationToken = default);

        Task<decimal> ExpenseAmountAsync(CancellationToken cancellationToken = default);

        Task<decimal> TotalAmountAsync(CancellationToken cancellationToken = default);

        Task<IReadOnlyList<ExpenseRecord>> GetTopExpensesAsync(int count = 3, CancellationToken cancellationToken = default);

        Task<bool> UpdateRecordAsync(FinancialRecord record, CancellationToken cancellationToken = default);

        Task DeleteAllRecordsAsync(CancellationToken cancellationToken = default);
    }
}
