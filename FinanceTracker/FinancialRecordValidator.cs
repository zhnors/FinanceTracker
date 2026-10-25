
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public static void ValidateSearchDate(DateTime date)
        {
            if (date > DateTime.Now) throw new ArgumentOutOfRangeException("Date is out of range");

            if (date == default) throw new ArgumentException("Date is out of range");
        }

        public static void ValidateSearchRange(DateTime from, DateTime to)
        {
            if (from > DateTime.Now || to > DateTime.Now) throw new ArgumentOutOfRangeException("Date is out of range");

            if (from > to) throw new ArgumentException("The start date must be earlier than or equal to the end date.");
        }

        public static void ValidateSearchCategory(string category) 
        {
            if (string.IsNullOrEmpty(category)) throw new ArgumentNullException();
        }

        

    }
}