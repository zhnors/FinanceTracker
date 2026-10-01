using FinanceTracker.Enums;

namespace FinanceTracker
{
    public class ExpenseRecord : FinancialRecord //витрата 
    {
        public ExpenseTypes ExpenseType { get; set; }

        public ExpenseRecord() : base() { }

        public ExpenseRecord(
            int id,
            string title, 
            decimal amount,
            DateTime date,
            string category, 
            string description,
            ExpenseTypes expenseType) 
                : base(id, title, amount, date, category, description)
        {
            ExpenseType = expenseType;
        }
    }
}
