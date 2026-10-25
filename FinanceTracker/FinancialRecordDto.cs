using FinanceTracker.Enums;

namespace FinanceTracker
{
    internal class FinancialRecordDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public RecordTypes RecordType { get; set; }     
        public FinancialRecordDto() { }

        public FinancialRecordDto(
            int id,
            string title,
            decimal amount,
            DateTime date,
            string category,
            string description,
            RecordTypes recordType)
        {
            Id = id;
            Title = title;
            Amount = amount;
            Date = date;
            Category = category;
            Description = description;
            RecordType = recordType;            
        }
 
    }
}
