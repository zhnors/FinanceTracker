namespace FinanceTracker.Api.Data.Entities
{
    public class FinancialRecordEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int RecordTypeId { get; set; }
        public RecordType RecordType { get; set; } = null!;
    }
}
