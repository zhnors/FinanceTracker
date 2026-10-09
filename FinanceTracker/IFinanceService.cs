
namespace FinanceTracker
{

    public delegate void LargeExpenseDelegate(FinancialRecord record);

    public interface IFinanceService
    {
        event LargeExpenseDelegate? LargeExpense;

        void AddRecord(FinancialRecord record);

        bool DeleteRecord(int id);

        IReadOnlyList<FinancialRecord> GetAllRecords();

        IReadOnlyList<FinancialRecord> SearchByCategory(string category);

        IReadOnlyList<FinancialRecord> SearchByDate(DateTime date);

        IReadOnlyList<FinancialRecord> SearchByDate(DateTime fromDate, DateTime toDate);

        FinancialRecord? GetRecordById(int id);

        decimal IncomeAmount();

        decimal ExpenseAmount();

        decimal TotalAmount();

        IReadOnlyList<ExpenseRecord> GetTopExpenses(int count = 3);

        void DeleteAllRecords();

        Task SaveToJsonFileAsync(string path);

        Task LoadFromJsonFileAsync(string path);

    }
}
