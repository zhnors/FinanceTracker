namespace FinanceTracker
{
    internal class Program
    {
        public void RunErrorTests(FinanceService financeService)
        {
            try
            {
                financeService.SearchByDate(new DateTime());
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
                financeService.SearchByDate(new DateTime(2026, 7, 23), new DateTime(2026, 7, 20));
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
                financeService.AddRecord(new ExpenseRecord
                {
                    Id = 6,
                    Title = "Transport",
                    Date = new DateTime(2026, 6, 20),
                    Category = "Transport",
                    Description = "Taxi and public transport",
                    ExpenseType = Enums.ExpenseTypes.Transport
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

        public void RunMainDemo(FinanceService financeService)
        {
            financeService.ShowAllRecords();

            var a = financeService.IncomeAmount();
            Console.WriteLine(a);

            a = financeService.ExpenseAmount();
            Console.WriteLine(a);

            a = financeService.TotalAmount();
            Console.WriteLine(a);

            financeService.SearchByCategory("Rent");

            financeService.SearchByDate(new DateTime(2026, 7, 20), DateTime.Now);

            financeService.ExpenseTop();      
        }

        public async Task TestJsonDemo(FinanceService financeService)
        {         
            await financeService.SaveToJsonFileAsync(@"D:\STUDING\Programming\self-study_C#\finance.json");

            financeService.DeleteAllRecords();
            financeService.ShowAllRecords();

            await financeService.LoadFromJsonFileAsync(@"D:\STUDING\Programming\self-study_C#\finance.json");

            financeService.ShowAllRecords();
        }

        static void Main(string[] args)
        {
            FinanceService financeService = new FinanceService();

            financeService.LargeExpense += financeService.LargeExpenseNotif;

            IncomeRecord income1 = new IncomeRecord
            {
                Id = 1,
                Title = "Salary",
                Amount = 30000,
                Date = DateTime.Now,
                Category = "Salary",
                Description = "Monthly salary" ,
                IncomeType = Enums.IncomeTypes.Salary
            };

            IncomeRecord income2 = new IncomeRecord
            {
                Id = 2,
                Title = "Freelance",
                Amount = 8000,
                Date = new DateTime(2026,7,20),
                Category = "Freelance",
                Description = "Small freelance project" ,
                IncomeType = Enums.IncomeTypes.Freelance
            };

            ExpenseRecord expense1 = new ExpenseRecord
            {
                Id = 3,
                Title = "Rent",
                Amount = 12000,
                Date = DateTime.Now,
                Category = "Rent",
                Description = "Monthly apartment rent" ,
                ExpenseType =Enums.ExpenseTypes.Rent
            };

            ExpenseRecord expense2 = new ExpenseRecord
            {
                Id = 4,
                Title = "Food",
                Amount = 2500,
                Date = new DateTime(2026, 7, 1),
                Category = "Food",
                Description = "Groceries" ,
                ExpenseType = Enums.ExpenseTypes.Food
            };

            ExpenseRecord expense3 = new ExpenseRecord
            {
                Id = 5,
                Title = "Transport",
                Amount = 900,
                Date = new DateTime(2026, 6, 20),
                Category = "Transport",
                Description = "Taxi and public transport" ,
                ExpenseType = Enums.ExpenseTypes.Transport
            };

            financeService.AddRecord(income1);
            financeService.AddRecord(income2);

            financeService.AddRecord(expense1);
            financeService.AddRecord(expense2);
            financeService.AddRecord(expense3);
        }
    }

}
