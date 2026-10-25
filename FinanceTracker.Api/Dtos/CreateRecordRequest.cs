using FinanceTracker.Enums;
using System.ComponentModel.DataAnnotations;


namespace FinanceTracker.Api.Dtos
{
    public class CreateRecordRequest : IValidatableObject
    {    
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Date > DateTime.Now)
                yield return new ValidationResult(
                    "Date can’t be in the future",
                    new[] {nameof(Date)});
        }
    }
}
