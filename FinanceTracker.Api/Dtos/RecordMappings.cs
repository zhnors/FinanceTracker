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
                    RecordType = RecordTypes.Income
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
                    RecordType = RecordTypes.Expense
                };
                return recordResponse;
            }
            else throw new ArgumentException($"Unknown record type: {record.GetType().Name}");

        }

        public static FinancialRecord ToRecord(this CreateRecordRequest request)
        {
            if (request.RecordType == RecordTypes.Income)
            {
                IncomeRecord incomeRecord = new IncomeRecord
                {               
                    Title = request.Title,
                    Amount = request.Amount,
                    Date = request.Date.Value,
                    Category = request.Category,
                    Description = request.Description
                };
                return incomeRecord;
            }
            else if (request.RecordType == RecordTypes.Expense)
            {
                ExpenseRecord expenseRecord = new ExpenseRecord
                {
                    Title = request.Title,
                    Amount = request.Amount,
                    Date = request.Date.Value,
                    Category = request.Category,
                    Description = request.Description
                };
                return expenseRecord;
            }
            else throw new ArgumentException($"Unknown record type: {request.RecordType}");
        }

        public static FinancialRecord ToRecord(this UpdateRecordRequest request, FinancialRecord existing)
        {
            if (existing is IncomeRecord incomeRecord)
            {
                IncomeRecord updatedIncome = new IncomeRecord
                {
                    Id = incomeRecord.Id,
                    Title = request.Title,
                    Amount = request.Amount,
                    Date = request.Date.Value,
                    Category = request.Category,
                    Description = request.Description
                };
                return updatedIncome;
            }
            else if (existing is ExpenseRecord expenseRecord)
            {
                ExpenseRecord updatedExpense = new ExpenseRecord
                {
                    Id = expenseRecord.Id,
                    Title = request.Title,
                    Amount = request.Amount,
                    Date = request.Date.Value,
                    Category = request.Category,
                    Description = request.Description
                };
                return updatedExpense;
            }
            else throw new ArgumentException($"Unknown record type: {existing.GetType().Name}");
        }

    }
}
