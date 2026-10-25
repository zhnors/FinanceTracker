using FinanceTracker.Enums;

namespace FinanceTracker
{
    public class IncomeRecord : FinancialRecord //дохід
    {
        public IncomeRecord():base() { }

        public IncomeRecord(
            int id,
            string title,
            decimal amount,
            DateTime date,
            string category,
            string description)
            : base(id, title, amount, date, category, description) { }       
    }
}
