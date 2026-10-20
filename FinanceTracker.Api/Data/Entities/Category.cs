namespace FinanceTracker.Api.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int RecordTypeId { get; set; }
        public RecordType RecordType { get; set; } = null!;
    }
}
