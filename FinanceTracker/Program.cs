namespace FinanceTracker
{
    internal class Program
    {

        public static void LargeExpenseNotif(FinancialRecord record)
        {
            Console.WriteLine($"Large expense detected: {record.Category} - {record.Amount}");
        }

        public static async Task RunErrorTests(IFinanceService financeService)
        {
            try
            {
                await financeService.SearchByDateAsync(new DateTime());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {
                await financeService.SearchByDateAsync(new DateTime(2026, 7, 23), new DateTime(2026, 7, 20));
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {
                await financeService.AddRecordAsync(new ExpenseRecord
                {
                    Id = 6,
                    Title = "Transport",
                    Date = new DateTime(2026, 6, 20),
                    Category = "Transport",
                    Description = "Taxi and public transport"
                });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        public static async Task RunMainDemo(IFinanceService financeService)
        {
            PrintRecords(await financeService.GetAllRecordsAsync());

            var a = await financeService.IncomeAmountAsync();
            Console.WriteLine(a);

            a = await financeService.ExpenseAmountAsync();
            Console.WriteLine(a);

            a = await financeService.TotalAmountAsync();
            Console.WriteLine(a);

            PrintRecords(await financeService.SearchByCategoryAsync("Rent"));

            PrintRecords(await financeService.SearchByDateAsync(new DateTime(2026, 7, 20), DateTime.Now));

            PrintRecords(await financeService.GetTopExpensesAsync(3));      
        }

        public static async Task TestJsonDemo(FinanceService financeService)
        {         
            await financeService.SaveToJsonFileAsync(@"D:\STUDING\Programming\self-study_C#\finance.json");

            await financeService.DeleteAllRecordsAsync();
            PrintRecords(await financeService.GetAllRecordsAsync());

            await financeService.LoadFromJsonFileAsync(@"D:\STUDING\Programming\self-study_C#\finance.json");

            PrintRecords(await financeService.GetAllRecordsAsync());
        }

        public static void PrintRecords(IEnumerable<FinancialRecord> records)
        {
            foreach (var record in records)
            {
                Console.WriteLine(record.Id + "\n" + record.Title + "\n" + record.Category + "\n" + record.Amount + "\n" + record.Date + "\n" + record.Description + "\n");
                Console.WriteLine();
            }
        }

        static async Task Main(string[] args)
        {
            FinanceService financeService = new FinanceService();

            financeService.LargeExpense += LargeExpenseNotif;

            IncomeRecord income1 = new IncomeRecord
            {
                Id = 1,
                Title = "Salary",
                Amount = 30000,
                Date = DateTime.Now,
                Category = "Salary",
                Description = "Monthly salary" 
            };

            IncomeRecord income2 = new IncomeRecord
            {
                Id = 2,
                Title = "Freelance",
                Amount = 8000,
                Date = new DateTime(2026,7,20),
                Category = "Freelance",
                Description = "Small freelance project" 
            };

            ExpenseRecord expense1 = new ExpenseRecord
            {
                Id = 3,
                Title = "Rent",
                Amount = 12000,
                Date = DateTime.Now,
                Category = "Rent",
                Description = "Monthly apartment rent" 
            };

            ExpenseRecord expense2 = new ExpenseRecord
            {
                Id = 4,
                Title = "Food",
                Amount = 2500,
                Date = new DateTime(2026, 7, 1),
                Category = "Food",
                Description = "Groceries" 
            };

            ExpenseRecord expense3 = new ExpenseRecord
            {
                Id = 5,
                Title = "Transport",
                Amount = 900,
                Date = new DateTime(2026, 6, 20),
                Category = "Transport",
                Description = "Taxi and public transport" 
            };

            await financeService.AddRecordAsync(income1);
            await financeService.AddRecordAsync(income2);

            await financeService.AddRecordAsync(expense1);
            await financeService.AddRecordAsync(expense2);
            await financeService.AddRecordAsync(expense3);

            await RunMainDemo(financeService);
            await RunErrorTests(financeService);
            await TestJsonDemo(financeService);
        }
    }

}
