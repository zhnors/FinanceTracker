using FinanceTracker.Enums;

namespace FinanceTracker
{
    internal class FinancialRecordDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public RecordTypes RecordType { get; set; }
        public IncomeTypes? IncomeType { get; set; }
        public ExpenseTypes? ExpenseType { get; set; }

        public FinancialRecordDto() { }

        public FinancialRecordDto(
            int id,
            string title,
            decimal amount,
            DateTime date,
            string category,
            string description,
            RecordTypes recordType,
            IncomeTypes? incomeType,
            ExpenseTypes? expenseType)
        {
            Id = id;
            Title = title;
            Amount = amount;
            Date = date;
            Category = category;
            Description = description;
            RecordType = recordType;
            IncomeType = incomeType;
            ExpenseType = expenseType;
        }
 
    }
}
