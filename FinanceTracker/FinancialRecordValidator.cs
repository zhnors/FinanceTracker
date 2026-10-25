
namespace FinanceTracker
{
    public static class FinancialRecordValidator
    {
        public static void Validate(FinancialRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            if (string.IsNullOrWhiteSpace(record.Title)) throw new ArgumentException(nameof(record.Title));

            if (string.IsNullOrWhiteSpace(record.Category)) throw new ArgumentException(nameof(record.Category));

            if (record.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(record.Amount));

            if (record.Date > DateTime.Now) throw new ArgumentOutOfRangeException(nameof(record.Date));

            if (record.Date == default) throw new ArgumentOutOfRangeException(nameof(record.Date));
        }
    }
}
