using FinanceTracker.Enums;
using Xunit;
using System;

namespace FinanceTracker.Tests
{
    public class FinanceServiceTests
    {
        private readonly FinanceService service;

        public FinanceServiceTests()
        {
            service = new FinanceService();
        }

        private static IncomeRecord CreateIncome(int id = 1, decimal amount = 30000, string title = "Salary")
        {
            return new IncomeRecord
            {
                Id = id,
                Title = title,
                Amount = amount,
                Date = new DateTime(2026, 7, 20),
                Category = "Salary",
                Description = "Monthly salary",
                IncomeType = IncomeTypes.Salary
            };
        }

        private static ExpenseRecord CreateExpense(int id = 2, decimal amount = 12000, string title = "Rent")
        {
            return new ExpenseRecord
            {
                Id = id,
                Title = title,
                Amount = amount,
                Date = new DateTime(2026, 7, 20),
                Category = "Rent",
                Description = "Monthly rent",
                ExpenseType = ExpenseTypes.Rent
            };
        }

        [Fact]
        public void AddRecord_ValidIncome_AddsIncome()
        {
            //Arrange
            IncomeRecord income = CreateIncome();

            //Act
            service.AddRecord(income);

            //Assert
            decimal result = service.IncomeAmount();

            Assert.Equal(30000, result);
        }

        [Fact]
        public void AddRecord_ValidExpense_AddsExpense()
        {
            //Arrange
            ExpenseRecord expense = CreateExpense();

            //Act
            service.AddRecord(expense);

            //Assert
            decimal result = service.ExpenseAmount();

            Assert.Equal(12000, result);
        }

        [Fact]
        public void AddRecord_NullRecord_ThrowsArgumentNullException()
        {
            //Arrange
            FinancialRecord record = null!;

            //Act
            //Assert

            Assert.Throws<ArgumentNullException>(()=> service.AddRecord(record));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void AddRecord_InvalidTitle_ThrowsArgumentException(string title)
        {
            //Arrange
            var record = CreateIncome(title: title);

            //Act
            //Assert

            Assert.Throws<ArgumentException>(() => service.AddRecord(record));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void AddRecord_InvalidCategory_ThrowsArgumentException(string category)
        {
            //Arrange
            FinancialRecord record = CreateIncome();

            record.Category = category;

            //Act
            //Assert

            Assert.Throws<ArgumentException>(() => service.AddRecord(record));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void AddRecord_InvalidDescription_ThrowsArgumentException(string description)
        {
            //Arrange
            var record = CreateExpense();

            record.Description = description;

            //Act
            //Assert

            Assert.Throws<ArgumentException>(() => service.AddRecord(record));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void AddRecord_InvalidAmount_ThrowsArgumentOutOfRangeException(int amount)
        {
            //Arrange
            var record = CreateExpense(amount : amount);

            //Act
            //Assert

            Assert.Throws<ArgumentOutOfRangeException>(() => service.AddRecord(record));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-25)]
        public void AddRecord_InvalidId_ThrowsArgumentOutOfRangeException(int id)
        {
            //Arrange
            var record = CreateExpense(id: id);

            //Act
            //Assert

            Assert.Throws<ArgumentOutOfRangeException>(() => service.AddRecord(record));
        }

        [Fact]
        public void AddRecord_DefaultDate_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            var record = CreateExpense();

            record.Date = default;

            //Act
            //Assert

            Assert.Throws<ArgumentOutOfRangeException>(() => service.AddRecord(record));
        }

        [Fact]
        public void AddRecord_FutureDate_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            var record = CreateExpense();

            record.Date = DateTime.Now.AddDays(1);

            //Act
            //Assert

            Assert.Throws<ArgumentOutOfRangeException>(() => service.AddRecord(record));
        }

        [Fact]
        public void AddRecord_BigExpense_RaisesLargeExpenseEvent()
        {
            bool isRaised = false;

            service.LargeExpense += record =>
            {
                isRaised = true;
            };

            service.AddRecord(CreateExpense(amount:15000));

            Assert.True(isRaised);
        }

        [Fact]
        public void DeleteRecord_ExistingRecord_ReturnsTrue()
        {
            var record = CreateExpense(id : 1);

            service.AddRecord(record);


            Assert.True(service.DeleteRecord(1));
        }

        [Fact]
        public void DeleteRecord_NotExistingRecord_ReturnsFalse()
        {
            var record = CreateExpense(id: 1);

            service.AddRecord(record);


            Assert.False(service.DeleteRecord(99));
        }

        [Fact]
        public void IncomeAmount_ReturnsCorrectSum()
        {
            var firstIncome = CreateIncome(id: 1,amount: 30000);

            var secondIncome = CreateIncome(id: 2, amount: 6000);

            service.AddRecord(firstIncome);

            service.AddRecord(secondIncome);

            decimal expected = 36000;


            var result = service.IncomeAmount();


            Assert.Equal(expected, result);

        }

        [Fact]
        public void ExpenseAmount_ReturnsCorrectSum()
        {
            var firstExpense = CreateExpense(id: 1, amount: 12000);

            var secondExpense = CreateExpense(id: 2, amount: 2500);

            service.AddRecord(firstExpense);

            service.AddRecord(secondExpense);

            decimal expected = 14500;

            var result = service.ExpenseAmount();

            Assert.Equal(expected, result);

        }

        [Fact]
        public void TotalAmount_ReturnsIncomeMinusExpenses()
        {
            var income = CreateIncome(id: 1, amount: 38000);

            var expense = CreateExpense(id: 2, amount: 14500);

            service.AddRecord(income);

            service.AddRecord(expense);

            decimal expected = 23500;

            var result = service.TotalAmount();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void DeleteAllRecords_ClearsRecords()
        {
            var income = CreateIncome(id: 1, amount: 38000);

            var expense = CreateExpense(id: 2, amount: 14500);

            service.AddRecord(income);

            service.AddRecord(expense);

            service.DeleteAllRecords();

            Assert.Equal(0, service.IncomeAmount());

            Assert.Equal(0, service.ExpenseAmount());

            Assert.Equal(0, service.TotalAmount());
        }

        [Fact]
        public async Task SaveToJsonFileAsync_EmptyPath_ThrowsArgumentException() 
        {
            service.AddRecord(CreateIncome());

            service.AddRecord(CreateExpense());

            await Assert.ThrowsAsync<ArgumentException>(() =>  service.SaveToJsonFileAsync("") );
        }

        [Fact]
        public async Task SaveToJsonFileAsync_CreatesNotEmptyFile() 
        {
            service.AddRecord(CreateIncome());

            service.AddRecord(CreateExpense());

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

            try
            {
                await service.SaveToJsonFileAsync(path);

                Assert.True(File.Exists(path));

                string json = await File.ReadAllTextAsync(path);

                Assert.False(string.IsNullOrWhiteSpace(json));
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }  
            }    
        }

        [Fact]
        public async Task LoadFromJsonFileAsync_FileDoesNotExist_ThrowsFileNotFoundException()
        {
            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

            await Assert.ThrowsAsync<FileNotFoundException>(() => service.LoadFromJsonFileAsync(path));
        }

        [Fact]
        public async Task LoadFromJsonFileAsync_EmptyFile_ThrowsInvalidDataException()
        {
            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");            

            try
            {
                await File.WriteAllTextAsync(path,"");

                await Assert.ThrowsAsync<InvalidDataException>(() => service.LoadFromJsonFileAsync(path));
            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }

        [Fact]
        public async Task LoadFromJsonFileAsync_RestoresIncomeAndExpenseAfterSaveAndLoad()
        {
            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

            service.AddRecord(CreateIncome(id: 1, amount: 3000));

            service.AddRecord(CreateExpense(id:2, amount: 1000));

            try
            {
                await service.SaveToJsonFileAsync(path);

                service.DeleteAllRecords();

                await service.LoadFromJsonFileAsync(path);

                Assert.Equal(3000, service.IncomeAmount());

                Assert.Equal(1000, service.ExpenseAmount());

                Assert.Equal(2000, service.TotalAmount());

            }
            finally
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }           
        }

    }
}
