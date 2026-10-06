
namespace FinanceTracker
{

    public delegate void LargeExpenseDelegate(FinancialRecord record);

    public interface IFinanceService
    {
        event LargeExpenseDelegate? LargeExpense;

        void AddRecord(FinancialRecord record);

        bool DeleteRecord(int id);

        void ShowAllRecords();

        void SearchByCategory(string category);

        void SearchByDate(DateTime date);

        void SearchByDate(DateTime fromDate, DateTime toDate);

        decimal IncomeAmount();

        decimal ExpenseAmount();

        decimal TotalAmount();

        void ExpenseTop();

        void DeleteAllRecords();

        Task SaveToJsonFileAsync(string path);

        Task LoadFromJsonFileAsync(string path);

    }
}
