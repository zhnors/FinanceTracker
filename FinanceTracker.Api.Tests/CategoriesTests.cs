using System.Net;
using System.Net.Http.Json;
using FinanceTracker.Api.Dtos;
using Microsoft.AspNetCore.Http;

namespace FinanceTracker.Api.Tests;

public class CategoriesTests : IClassFixture<FinanceApiFactory>
{
    private readonly HttpClient _client;

    public CategoriesTests(FinanceApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ReturnsSeededCategories()
    {
        var response = await _client.GetAsync("/categories", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var categories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>(
            TestContext.Current.CancellationToken);

        Assert.NotNull(categories);
        Assert.Equal(10, categories.Count);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/categories/999", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

}

