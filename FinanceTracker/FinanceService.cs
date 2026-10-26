using FinanceTracker.Enums;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinanceTracker
{
    public class FinanceService : IFinanceService, IRecordFileStorage
    {
        public event LargeExpenseDelegate? LargeExpense;

        List<FinancialRecord> RecordList = new List<FinancialRecord>();

        public Task AddRecordAsync(FinancialRecord record)
        {
            ArgumentNullException.ThrowIfNull(record);

            if (record.Id < 0) throw new ArgumentOutOfRangeException(nameof(record.Id));

            if (record.Id == 0) 
            {
                if (RecordList.Count == 0) record.Id = 1;

                else record.Id = RecordList.Max(r => r.Id) + 1;
            }

            if (RecordList.Exists(r => r.Id == record.Id)) throw new InvalidOperationException($"Record with Id {record.Id} already exists");

            FinancialRecordValidator.Validate(record);

            RecordList.Add(record);

            if (record is ExpenseRecord && record.Amount >= FinanceRules.LargeExpenseLimit)
            {
                LargeExpense?.Invoke(record);
            }

            return Task.CompletedTask;
        }

        public Task<bool> DeleteRecordAsync(int id)
        {
            var isFound = false;
            var recordToDelete = RecordList.FirstOrDefault(record => record.Id == id);

            if (recordToDelete != null)
            {
                isFound = true;

                RecordList.Remove(recordToDelete);
            }

            return Task.FromResult(isFound);
        }

        public Task<IReadOnlyList<FinancialRecord>> GetAllRecordsAsync()
        {
            IReadOnlyList<FinancialRecord> result = RecordList.ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<FinancialRecord>> SearchByCategoryAsync(string category)
        {
            FinancialRecordValidator.ValidateSearchCategory(category);

            IReadOnlyList<FinancialRecord> result = RecordList.Where(record => record.Category == category).ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime date)
        {
            FinancialRecordValidator.ValidateSearchDate(date);
            
            IReadOnlyList<FinancialRecord> result = RecordList.Where(record => record.Date.Date == date.Date).ToList();

            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<FinancialRecord>> SearchByDateAsync(DateTime fromDate, DateTime toDate)
        {
            FinancialRecordValidator.ValidateSearchRange(fromDate, toDate);

            IReadOnlyList<FinancialRecord> result = RecordList.Where(record => record.Date.Date >= fromDate.Date && record.Date.Date <= toDate.Date).ToList();
            
            return Task.FromResult(result);
        }

        public Task<FinancialRecord?> GetRecordByIdAsync(int id)
        {
            FinancialRecord? result = RecordList.FirstOrDefault(record => record.Id == id);

            return Task.FromResult(result);
        }

        public Task<decimal> IncomeAmountAsync()
        {
            var income = RecordList.OfType<IncomeRecord>().Sum(r => r.Amount);              

            return Task.FromResult(income);
        }

        public Task<decimal> ExpenseAmountAsync()
        {
            var expense = RecordList.OfType<ExpenseRecord>().Sum(r => r.Amount);              

            return Task.FromResult(expense);
        }

        public Task<decimal> TotalAmountAsync()
        {
            var total = RecordList.Sum(r => r is ExpenseRecord ? -r.Amount : r.Amount);

            return Task.FromResult(total);
        }

        public Task<IReadOnlyList<ExpenseRecord>> GetTopExpensesAsync(int count)
        {
            IReadOnlyList<ExpenseRecord> result = RecordList
                .OfType<ExpenseRecord>()
                .OrderByDescending(r => r.Amount)
                .Take(count)
                .ToList();

            return Task.FromResult(result);
        }

        public Task<bool> UpdateRecordAsync(FinancialRecord record)
        {
            var index = RecordList.FindIndex(r => r.Id == record.Id);

            if (index == -1) return Task.FromResult(false);

            FinancialRecordValidator.Validate(record);

            RecordList[index] = record;

            return Task.FromResult(true);
        }

        public Task DeleteAllRecordsAsync()
        {
            RecordList.Clear();

            return Task.CompletedTask;
        }

        public async Task SaveToJsonFileAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException();

            var dtoRecordList = ToDto(RecordList);

            var json = JsonSerializer.Serialize(dtoRecordList, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(path, json);
        }

        public async Task LoadFromJsonFileAsync(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException();
            if (!File.Exists(path)) throw new FileNotFoundException();

            var json = await File.ReadAllTextAsync(path);

            if (string.IsNullOrWhiteSpace(json)) throw new InvalidDataException("File is empty ");

            var dtoRecordList = JsonSerializer.Deserialize<List<FinancialRecordDto>>(json);

            if (dtoRecordList == null) throw new FormatException();

            var recordList = FromDto(dtoRecordList);

            if (recordList != null) RecordList = recordList;
        }

        private List<FinancialRecordDto> ToDto(List<FinancialRecord> financialRecords)
        {
            List<FinancialRecordDto> dtoRecords = new List<FinancialRecordDto>();

            foreach (var record in financialRecords)
            {
                if (record is IncomeRecord incomeRecord)
                {
                    FinancialRecordDto dtoRec = new FinancialRecordDto
                    {
                        Id = incomeRecord.Id,
                        Title = incomeRecord.Title,
                        Amount = incomeRecord.Amount,
                        Date = incomeRecord.Date,
                        Category = incomeRecord.Category,
                        Description = incomeRecord.Description,
                        RecordType = RecordTypes.Income
                    };

                    dtoRecords.Add(dtoRec);
                }
                else if (record is ExpenseRecord expenseRecord)
                {
                    FinancialRecordDto dtoRec = new FinancialRecordDto
                    {
                        Id = expenseRecord.Id,
                        Title = expenseRecord.Title,
                        Amount = expenseRecord.Amount,
                        Date = expenseRecord.Date,
                        Category = expenseRecord.Category,
                        Description = expenseRecord.Description,
                        RecordType = RecordTypes.Expense
                    };

                    dtoRecords.Add(dtoRec);
                }
                else throw new FormatException();
            }

            return dtoRecords;
        }

        private List<FinancialRecord> FromDto(List<FinancialRecordDto> dtoRecords)
        {
            List<FinancialRecord> financialRecords = new List<FinancialRecord>();

            foreach (var record in dtoRecords)
            {
                if (record.RecordType == RecordTypes.Income)
                {
                    IncomeRecord incomeRecord = new IncomeRecord
                    {
                        Id = record.Id,
                        Title = record.Title,
                        Amount = record.Amount,
                        Date = record.Date,
                        Category = record.Category,
                        Description = record.Description
                    };

                    financialRecords.Add(incomeRecord);
                }
                else if (record.RecordType == RecordTypes.Expense)
                {
                    ExpenseRecord expenseRecord = new ExpenseRecord
                    {
                        Id = record.Id,
                        Title = record.Title,
                        Amount = record.Amount,
                        Date = record.Date,
                        Category = record.Category,
                        Description = record.Description
                    };

                    financialRecords.Add(expenseRecord);
                }
                else throw new FormatException();
            }

            return financialRecords;
        }
    }
}
