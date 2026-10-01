namespace FinanceTracker
{
    public class FinancialRecord
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public FinancialRecord(){ }

        public FinancialRecord(
            int id,
            string title,
            decimal amount,
            DateTime date,
            string category,
            string description)
        {
            Id = id;
            Title = title;
            Amount = amount;
            Date = date;
            Category = category;
            Description = description;
        }
    }
}
