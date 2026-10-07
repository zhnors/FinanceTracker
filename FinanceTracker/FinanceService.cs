using FinanceTracker.Enums;
using System.Text.Json;

namespace FinanceTracker
{
    public class FinanceService : IFinanceService
    {
        readonly decimal largeExpenseLimit = 10000;

        public event LargeExpenseDelegate? LargeExpense;

        List<FinancialRecord> RecordList = new List<FinancialRecord>();

        public void AddRecord(FinancialRecord record)
        {
            ValidateRecord(record);

            RecordList.Add(record);

            if (record is ExpenseRecord && record.Amount >= largeExpenseLimit)
            {
                LargeExpense?.Invoke(record);
            }
        }

        public bool DeleteRecord(int id)
        {
            var isFound = false;
            var recordToDelete = RecordList.FirstOrDefault(record => record.Id == id);

            if (recordToDelete != null)
            {
                isFound = true;

                RecordList.Remove(recordToDelete);
            }

            return isFound;
        }

        public IReadOnlyList<FinancialRecord> GetAllRecords()
        {
            return RecordList.ToList();
        }

        public IReadOnlyList<FinancialRecord> SearchByCategory(string category)
        {
           if (string.IsNullOrEmpty(category)) throw new ArgumentNullException();

           return RecordList.Where(record => record.Category == category).ToList();       
        }

        public IReadOnlyList<FinancialRecord> SearchByDate(DateTime date)
        {
            if (date > DateTime.Now) throw new ArgumentOutOfRangeException("Date is out of range");
            if (date == default) throw new ArgumentException("Date is out of range");

            return RecordList.Where(record => record.Date.Date == date.Date).ToList();             
        }

        public IReadOnlyList<FinancialRecord> SearchByDate(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > DateTime.Now || toDate > DateTime.Now) throw new ArgumentOutOfRangeException("Date is out of range");
            if (fromDate > toDate) throw new ArgumentException("The start date must be earlier than or equal to the end date.");

            return RecordList.Where(record => record.Date.Date >= fromDate.Date && record.Date.Date <= toDate.Date).ToList();          
        }

        public decimal IncomeAmount()
        {
            IEnumerable<FinancialRecord> incomeList = RecordList.OfType<IncomeRecord>();

            if (!incomeList.Any())
            {
                return 0;
            }

            decimal income = 0;

            foreach (var record in incomeList)
            {
                income += record.Amount;
            }

            return income;
        }

        public decimal ExpenseAmount()
        {
            IEnumerable<FinancialRecord> expenseList = RecordList.OfType<ExpenseRecord>();

            if (!expenseList.Any())
            {
                return 0;
            }

            decimal total = 0;

            foreach (var record in expenseList)
            {
                total += record.Amount;
            }

            return total;
        }

        public decimal TotalAmount()
        {
            decimal income = 0;

            foreach (var record in RecordList)
            {
                if (record is ExpenseRecord)
                {
                    income -= record.Amount;
                }

                else income += record.Amount;
            }

            return income;

        }

        public IReadOnlyList<ExpenseRecord> GetTopExpenses(int count)
        {
                return RecordList
                .OfType<ExpenseRecord>()
                .OrderByDescending(r => r.Amount)
                .Take(count)
                .ToList();   
        }

        public void DeleteAllRecords()
        {
            RecordList.Clear();
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
                        RecordType = RecordTypes.Income,
                        IncomeType = incomeRecord.IncomeType,
                        ExpenseType = null
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
                        RecordType = RecordTypes.Expense,
                        IncomeType = null,
                        ExpenseType = expenseRecord.ExpenseType
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
                    if (record.IncomeType == null)
                    {
                        throw new FormatException("IncomeType is missing");
                    }

                    IncomeRecord incomeRecord = new IncomeRecord
                    {
                        Id = record.Id,
                        Title = record.Title,
                        Amount = record.Amount,
                        Date = record.Date,
                        Category = record.Category,
                        Description = record.Description,
                        IncomeType = record.IncomeType.Value
                    };

                    financialRecords.Add(incomeRecord);
                }
                else if (record.RecordType == RecordTypes.Expense)
                {
                    if (record.ExpenseType == null)
                    {
                        throw new FormatException("ExpenseType is missing");
                    }

                    ExpenseRecord expenseRecord = new ExpenseRecord
                    {
                        Id = record.Id,
                        Title = record.Title,
                        Amount = record.Amount,
                        Date = record.Date,
                        Category = record.Category,
                        Description = record.Description,
                        ExpenseType = record.ExpenseType.Value
                    };

                    financialRecords.Add(expenseRecord);
                }
                else throw new FormatException();
            }

            return financialRecords;
        }

        private static void ValidateRecord(FinancialRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            if (string.IsNullOrWhiteSpace(record.Title)) throw new ArgumentException(nameof(record.Title));

            if (string.IsNullOrWhiteSpace(record.Description)) throw new ArgumentException(nameof(record.Description));

            if (string.IsNullOrWhiteSpace(record.Category)) throw new ArgumentException(nameof(record.Category));

            if (record.Id <= 0) throw new ArgumentOutOfRangeException(nameof(record.Id));

            if (record.Amount <= 0) throw new ArgumentOutOfRangeException(nameof(record.Amount));

            if (record.Date > DateTime.Now) throw new ArgumentOutOfRangeException(nameof(record.Date));

            if (record.Date == default) throw new ArgumentOutOfRangeException(nameof(record.Date));
        }
    }
}
