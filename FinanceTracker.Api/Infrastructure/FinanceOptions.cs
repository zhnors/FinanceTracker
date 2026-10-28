using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Api.Infrastructure;

public class FinanceOptions
{
    public const string SectionName = "Finance";

    [Range(0.01, 1_000_000_000)]
    public decimal LargeExpenseLimit { get; set; } = FinanceRules.LargeExpenseLimit;

    [Range(1, int.MaxValue)]
    public int DemoUserId { get; set; } = 1;
}