using FinanceTracker.Enums;
using System.ComponentModel.DataAnnotations;


namespace FinanceTracker.Api.Dtos
{
    public class CreateRecordRequest : IValidatableObject
    {
        [Range(1, int.MaxValue)]
        public int Id { get; init; }

        [Required]
        [StringLength(100)]
        public string Title { get; init; } = string.Empty;

        [Range(0.01, 1_000_000_000)]
        public decimal Amount { get; init; }

        [Required]
        public DateTime? Date { get; init; }

        [Required]
        [StringLength(100)]
        public string Category { get; init; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Description { get; init; } = string.Empty;

        [Required]
        public RecordTypes? RecordType { get; init; }
        public IncomeTypes? IncomeType { get; init; }
        public ExpenseTypes? ExpenseType { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RecordType == RecordTypes.Income && IncomeType is null)
                yield return new ValidationResult(
                    "IncomeType is required for income records",
                    new[] { nameof(IncomeType) });

            if (RecordType == RecordTypes.Expense && ExpenseType is null)
                yield return new ValidationResult(
                    "ExpenseType is required for expense records",
                    new[] { nameof(ExpenseType) });

            if(Date > DateTime.Now)
                yield return new ValidationResult(
                    "Date can’t be in the future",
                    new[] {nameof(Date)});
        }


    }
}
