namespace FinanceTracker.Api.Data.Entities
{
    public class RecordType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<Category> Categories { get; set; } = new();
    }
}
