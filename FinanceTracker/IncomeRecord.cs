using FinanceTracker.Enums;

namespace FinanceTracker
{
    public class IncomeRecord : FinancialRecord //дохід
    {
        public IncomeTypes IncomeType { get; set; }

        public IncomeRecord():base() { }

        public IncomeRecord(
            int id,
            string title,
            decimal amount,
            DateTime date,
            string category,
            string description,
            IncomeTypes incomeType)
            : base(id, title, amount, date, category, description)
        {
            IncomeType = incomeType;
        }
    }
}
