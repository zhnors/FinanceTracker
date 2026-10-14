
using FinanceTracker.Enums;

namespace FinanceTracker.Api.Dtos

{
    public static class RecordMappings
    {
        public static RecordResponse ToResponse(this FinancialRecord record)
        {
            if (record is IncomeRecord incomeRecord)
            {
                RecordResponse recordResponse = new RecordResponse
                {
                    Id = incomeRecord.Id,
                    Title = incomeRecord.Title,
                    Amount = incomeRecord.Amount,
                    Date = incomeRecord.Date,
                    Category = incomeRecord.Category,
                    Description = incomeRecord.Description,
                    RecordType = RecordTypes.Income,
                    IncomeType = incomeRecord.IncomeType,
                    ExpenseType = null
                };
                return recordResponse;
            }
            else if (record is ExpenseRecord expenseRecord)
            {
                RecordResponse recordResponse = new RecordResponse
                {
                    Id = expenseRecord.Id,
                    Title = expenseRecord.Title,
                    Amount = expenseRecord.Amount,
                    Date = expenseRecord.Date,
                    Category = expenseRecord.Category,
                    Description = expenseRecord.Description,
                    RecordType = RecordTypes.Expense,
                    IncomeType = null,
                    ExpenseType = expenseRecord.ExpenseType
                };
                return recordResponse;
            }
            else throw new ArgumentException($"Unknown record type: {record.GetType().Name}");

        }
    }
}
