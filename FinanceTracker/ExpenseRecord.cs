using FinanceTracker.Enums;

namespace FinanceTracker
{
    public class ExpenseRecord : FinancialRecord //витрата 
    {    
        public ExpenseRecord() : base() { }

        public ExpenseRecord(
            int id,
            string title, 
            decimal amount,
            DateTime date,
            string category, 
            string description) 
                : base(id, title, amount, date, category, description) { }
    }
}
