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

        private static IncomeRecord CreateIncome(int id = 0, decimal amount = 30000, string title = "Salary", string category = "Salary")
        {
            return new IncomeRecord
            {
                Id = id,
                Title = title,
                Amount = amount,
                Date = new DateTime(2026, 7, 20),
                Category = category,
                Description = "Monthly salary"
            };
        }

        private static ExpenseRecord CreateExpense(int id = 0, decimal amount = 12000, string title = "Rent", string category = "Rent")
        {
            return new ExpenseRecord
            {
                Id = id,
                Title = title,
                Amount = amount,
                Date = new DateTime(2026, 7, 20),
                Category = category,
                Description = "Monthly rent"
            };
        }

        [Fact]
        public async Task AddRecord_ValidIncome_AddsIncome()
        {
            //Arrange
            IncomeRecord income = CreateIncome();

            //Act
            await service.AddRecordAsync(income);

            //Assert
            decimal result = await service.IncomeAmountAsync();

            Assert.Equal(30000, result);
        }

        [Fact]
        public async Task AddRecord_ValidExpense_AddsExpense()
        {
            //Arrange
            ExpenseRecord expense = CreateExpense();

            //Act
            await service.AddRecordAsync(expense);

            //Assert
            decimal result = await service.ExpenseAmountAsync();

            Assert.Equal(12000, result);
        }

        [Fact]
        public async Task AddRecord_NullRecord_ThrowsArgumentNullException()
        {
            //Arrange
            FinancialRecord record = null!;

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentNullException>(()=> service.AddRecordAsync(record));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddRecord_InvalidTitle_ThrowsArgumentException(string title)
        {
            //Arrange
            var record = CreateIncome(title: title);

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddRecordAsync(record));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddRecord_InvalidCategory_ThrowsArgumentException(string category)
        {
            //Arrange
            FinancialRecord record = CreateIncome();

            record.Category = category;

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddRecordAsync(record));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task AddRecord_InvalidDescription_ThrowsArgumentException(string description)
        {
            //Arrange
            var record = CreateExpense();

            record.Description = description;

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentException>(() => service.AddRecordAsync(record));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task AddRecord_InvalidAmount_ThrowsArgumentOutOfRangeException(int amount)
        {
            //Arrange
            var record = CreateExpense(amount : amount);

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddRecordAsync(record));
        }

        [Theory]
        [InlineData(-25)]
        public async Task AddRecord_InvalidId_ThrowsArgumentOutOfRangeException(int id)
        {
            //Arrange
            var record = CreateExpense(id: id);

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddRecordAsync(record));
        }

        [Fact]
        public async Task AddRecord_RecordWithoutId_GetsId() 
        {
            var record = CreateIncome();

            await service.AddRecordAsync(record);

            Assert.Equal(1, record.Id);
        }

        [Fact]
        public async Task AddRecord_TwoRecordsWithSameId_ThrowsInvalidOperationException() 
        {
            await service.AddRecordAsync(CreateIncome(id: 1));

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddRecordAsync(CreateExpense(id: 1)));
        }

        [Fact]
        public async Task AddRecord_RecordsWithoutId_GetSequentialIds()
        {
            var first = CreateIncome();
            var second = CreateExpense();

            await service.AddRecordAsync(first);
            await service.AddRecordAsync(second);

            Assert.Equal(1, first.Id);
            Assert.Equal(2, second.Id);
        }

        [Fact]
        public async Task AddRecord_DefaultDate_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            var record = CreateExpense();

            record.Date = default;

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddRecordAsync(record));
        }

        [Fact]
        public async Task AddRecord_FutureDate_ThrowsArgumentOutOfRangeException()
        {
            //Arrange
            var record = CreateExpense();

            record.Date = DateTime.Now.AddDays(1);

            //Act
            //Assert

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.AddRecordAsync(record));
        }

        [Fact]
        public async Task AddRecord_BigExpense_RaisesLargeExpenseEvent()
        {
            bool isRaised = false;

            service.LargeExpense += record =>
            {
                isRaised = true;
            };

            await service.AddRecordAsync(CreateExpense(amount:15000));

            Assert.True(isRaised);
        }

        [Fact]
        public async Task DeleteRecord_ExistingRecord_ReturnsTrue()
        {
            var record = CreateExpense(id : 1);

            await service.AddRecordAsync(record);


            Assert.True(await service.DeleteRecordAsync(1));
        }

        [Fact]
        public async Task DeleteRecord_NotExistingRecord_ReturnsFalse()
        {
            var record = CreateExpense(id: 1);

            await service.AddRecordAsync(record);


            Assert.False(await service.DeleteRecordAsync(99));
        }

        [Fact]
        public async Task GetAllRecords_ReturnsAllAddedRecords()
        {
            await service.AddRecordAsync(CreateExpense());

            await service.AddRecordAsync(CreateIncome());


            var result = await service.GetAllRecordsAsync();


            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetAllRecords_ReturnsCopy()
        {
            await service.AddRecordAsync(CreateIncome());

            await service.AddRecordAsync(CreateExpense());


            var result = await service.GetAllRecordsAsync();

            await service.DeleteAllRecordsAsync();


            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task SearchByCategory_ReturnsOnlyMatchingRecords() 
        {
            await service.AddRecordAsync(CreateExpense(id: 1, category: "Food"));

            await service.AddRecordAsync(CreateIncome(id: 2, category: "Bonus"));

            await service.AddRecordAsync(CreateIncome(id: 3, category: "Bonus"));  
            

            var result = await service.SearchByCategoryAsync("Bonus");


            Assert.Equal(2, result.Count);

            Assert.All(result, r => Assert.Equal("Bonus", r.Category));
        }

        [Fact]
        public async Task SearchByCategory_NoMatches_ReturnsEmptyList()
        {
            await service.AddRecordAsync(CreateExpense(category: "Food"));

            await service.AddRecordAsync(CreateIncome(category: "Bonus"));


            var result = await service.SearchByCategoryAsync("Rent");


            Assert.Empty(result);
        }

        [Fact]
        public async Task SearchByDate_Range_ReturnsRecordsWithinRange()
        {
            DateTime fromDate = new DateTime(2026, 10, 1);

            DateTime toDate = new DateTime(2026, 10, 7);

            var firstExpense = CreateExpense();

            firstExpense.Date = new DateTime(2026, 7, 20);

            var secondExpense = CreateExpense();

            secondExpense.Date = new DateTime(2026, 8, 20);

            var firstIncome = CreateIncome();

            firstIncome.Date = fromDate;

            var secondIncome = CreateIncome();

            secondIncome.Date = toDate;

            await service.AddRecordAsync(firstExpense);

            await service.AddRecordAsync(secondExpense);

            await service.AddRecordAsync(firstIncome);

            await service.AddRecordAsync(secondIncome);


            var result = await service.SearchByDateAsync(fromDate, toDate);


            Assert.Equal(2, result.Count);

            Assert.Equal(fromDate, result[0].Date);

            Assert.Equal(toDate, result[1].Date);
        }

        [Fact]
        public async Task GetRecordById_RecordNotFound_ReturnsNull()
        {
            await service.AddRecordAsync(CreateExpense(id: 1));

            await service.AddRecordAsync(CreateExpense(id: 2));

            Assert.Null(await service.GetRecordByIdAsync(3));
        }

        [Fact]
        public async Task GetRecordById_RecordFound_ReturnsRecord()
        {
            await service.AddRecordAsync(CreateExpense(id: 2));

            await service.AddRecordAsync(CreateExpense(id: 3));

            var result = await service.GetRecordByIdAsync(2);

            Assert.NotNull(result);

            Assert.Equal(2, result.Id);
        }

        [Fact]
        public async Task GetTopExpenses_ReturnsHighestFirst()
        {
            await service.AddRecordAsync(CreateExpense(id: 1, amount: 3000));

            await service.AddRecordAsync(CreateExpense(id: 2, amount: 6000));

            await service.AddRecordAsync(CreateExpense(id: 3, amount: 8000));

            await service.AddRecordAsync(CreateExpense(id: 4, amount: 10000));


            var result = await service.GetTopExpensesAsync(2);

            Assert.Equal(2, result.Count);

            Assert.Equal(10000, result[0].Amount);

            Assert.Equal(8000, result[1].Amount);
        }

        [Fact]
        public async Task IncomeAmount_ReturnsCorrectSum()
        {
            var firstIncome = CreateIncome(id: 1,amount: 30000);

            var secondIncome = CreateIncome(id: 2, amount: 6000);

            await service.AddRecordAsync(firstIncome);

            await service.AddRecordAsync(secondIncome);

            decimal expected = 36000;


            var result = await service.IncomeAmountAsync();


            Assert.Equal(expected, result);

        }

        [Fact]
        public async Task ExpenseAmount_ReturnsCorrectSum()
        {
            var firstExpense = CreateExpense(id: 1, amount: 12000);

            var secondExpense = CreateExpense(id: 2, amount: 2500);

            await service.AddRecordAsync(firstExpense);

            await service.AddRecordAsync(secondExpense);

            decimal expected = 14500;

            var result = await service.ExpenseAmountAsync();

            Assert.Equal(expected, result);

        }

        [Fact]
        public async Task TotalAmount_ReturnsIncomeMinusExpenses()
        {
            var income = CreateIncome(id: 1, amount: 38000);

            var expense = CreateExpense(id: 2, amount: 14500);

            await service.AddRecordAsync(income);

            await service.AddRecordAsync(expense);

            decimal expected = 23500;

            var result = await service.TotalAmountAsync();

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task UpdateRecord_ExistingRecord_ReplacesRecord()
        {           
            var income = CreateIncome(id: 16, amount: 16000);

            var newincome = CreateIncome(id: 16, amount: 26000);

            await service.AddRecordAsync(income);

            var result = await service.UpdateRecordAsync(newincome);

            var updated = await service.GetRecordByIdAsync(16);

            Assert.True(result);
            Assert.Equal(26000, updated!.Amount);
        }

        [Fact]
        public async Task UpdateRecord_NonExistentRecord_ReturnsFalse()
        {
            var newincome = CreateIncome(id: 16, amount: 26000);

            await service.AddRecordAsync(CreateIncome(id: 15, amount: 16000));

            var result = await service.UpdateRecordAsync(newincome);

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateRecord_InvalidAmount_ThrowsArgumentOutOfRangeException()
        {
            var income = CreateIncome(id: 16, amount: 16000);

            var newincome = CreateIncome(id: 16, amount: -26000);

            await service.AddRecordAsync(income);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => service.UpdateRecordAsync(newincome));

            Assert.Equal(16000, (await service.GetRecordByIdAsync(16))!.Amount);
        }

        [Fact]
        public async Task DeleteAllRecords_ClearsRecords()
        {
            var income = CreateIncome(id: 1, amount: 38000);

            var expense = CreateExpense(id: 2, amount: 14500);

            await service.AddRecordAsync(income);

            await service.AddRecordAsync(expense);

            await service.DeleteAllRecordsAsync();

            Assert.Equal(0, await service.IncomeAmountAsync());

            Assert.Equal(0, await service.ExpenseAmountAsync());

            Assert.Equal(0, await service.TotalAmountAsync());
        }

        [Fact]
        public async Task SaveToJsonFileAsync_EmptyPath_ThrowsArgumentException() 
        {
            await service.AddRecordAsync(CreateIncome());

            await service.AddRecordAsync(CreateExpense());

            await Assert.ThrowsAsync<ArgumentException>(() => service.SaveToJsonFileAsync("") );
        }

        [Fact]
        public async Task SaveToJsonFileAsync_CreatesNotEmptyFile() 
        {
            await service.AddRecordAsync(CreateIncome());

            await service.AddRecordAsync(CreateExpense());

            string path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.json");

            try
            {
                await service.SaveToJsonFileAsync(path);

                Assert.True(File.Exists(path));

                string json = await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken);

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
                await File.WriteAllTextAsync(path, "", TestContext.Current.CancellationToken);

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

            await service.AddRecordAsync(CreateIncome(id: 1, amount: 3000));

            await service.AddRecordAsync(CreateExpense(id:2, amount: 1000));

            try
            {
                await service.SaveToJsonFileAsync(path);

                await service.DeleteAllRecordsAsync();

                await service.LoadFromJsonFileAsync(path);

                Assert.Equal(3000, await service.IncomeAmountAsync());

                Assert.Equal(1000, await service.ExpenseAmountAsync());

                Assert.Equal(2000, await service.TotalAmountAsync());

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
