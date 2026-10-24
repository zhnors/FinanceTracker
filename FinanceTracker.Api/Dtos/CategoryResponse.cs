using FinanceTracker.Api.Data.Entities;

namespace FinanceTracker.Api.Dtos
{
    public class CategoryResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string RecordType { get; init; } = string.Empty;
    }
}
