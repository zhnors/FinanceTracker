using FinanceTracker.Api.Dtos;
using FinanceTracker.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace FinanceTracker.Api.Tests;

[Collection(nameof(ApiCollection))]
public class RecordsTests : IAsyncLifetime
{
    private readonly FinanceApiFactory _factory;
    private readonly HttpClient _client;

    public RecordsTests(FinanceApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async ValueTask InitializeAsync() => await _factory.ResetRecordsAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CreateRecordRequest ValidExpense(decimal amount = 1500, string category = "Food") => new()
    {
        Title = "Groceries",
        Amount = amount,
        Date = new DateTime(2026, 10, 1),
        Category = category,
        RecordType = RecordTypes.Expense
    };

    private async Task<RecordResponse> CreateAsync(CreateRecordRequest request)
    {
        var response = await _client.PostAsJsonAsync("/records", request, TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await response.Content.ReadFromJsonAsync<RecordResponse>(TestJson.Options,
            TestContext.Current.CancellationToken);

        return created!;
    }

    [Fact]
    public async Task Create_ValidRecord_CanBeRetrieved()
    {
        var created = await CreateAsync(ValidExpense());

        var response = await _client.GetAsync($"/records/{created.Id}",
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var record = await response.Content.ReadFromJsonAsync<RecordResponse>(TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.Equal("Groceries", record?.Title);
        Assert.Equal(RecordTypes.Expense, record?.RecordType);
    }

    [Fact]
    public async Task Create_NegativeAmount_ReturnsValidationProblem()
    {
        var response = await _client.PostAsJsonAsync("/records", ValidExpense(-100), TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.True(error?.Errors.ContainsKey("Amount"));
    }

    [Fact]
    public async Task Create_UnknownCategory_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/records", ValidExpense(category: "Test"), TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.Contains("Test", problem?.Detail);
    }

    [Fact]
    public async Task Update_ExistingRecord_ChangesRecord()
    {
        var created = await CreateAsync(ValidExpense());

        var request = new UpdateRecordRequest
        {
            Title = "Updated",
            Amount = 2000,
            Date = new DateTime(2026, 10, 1),
            Category = "Food"
        };

        var response = await _client.PutAsJsonAsync($"/records/{created.Id}", request, TestJson.Options,
            TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var check = await _client.GetAsync($"/records/{created.Id}",
            TestContext.Current.CancellationToken);

        var result = await check.Content.ReadFromJsonAsync<RecordResponse>(TestJson.Options, TestContext.Current.CancellationToken);

        Assert.Equal("Updated", result!.Title);

    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var request = new UpdateRecordRequest
        {
            Title = "Updated",
            Amount = 2000,
            Date = new DateTime(2026, 10, 1),
            Category = "Food"
        };

        var response = await _client.PutAsJsonAsync("/records/999", request, TestJson.Options, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingRecord_RemovesRecord()
    {
        var created = await CreateAsync(ValidExpense());

        var response = await _client.DeleteAsync($"/records/{created.Id}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var empty = await _client.GetAsync($"/records/{created.Id}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, empty.StatusCode);
    }


    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync("/records/999", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Summary_ReturnsTotals()
    {
        await CreateAsync(ValidExpense(5000));

        await CreateAsync(new CreateRecordRequest
        {
            Title = "Updated",
            Amount = 15000,
            Date = new DateTime(2026, 10, 1),
            Category = "Salary",
            RecordType = RecordTypes.Income
        });

        var response = await _client.GetAsync("/records/summary", TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<SummaryResponse>(TestJson.Options, TestContext.Current.CancellationToken);

        Assert.Equal(10000, result!.Total);
    }

}