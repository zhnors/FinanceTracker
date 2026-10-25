using FinanceTracker.Enums;

namespace FinanceTracker.Api.Dtos;

public class RecordResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public RecordTypes RecordType { get; init; }
}