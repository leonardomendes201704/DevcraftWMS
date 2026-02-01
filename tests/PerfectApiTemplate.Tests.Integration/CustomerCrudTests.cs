using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Infrastructure.Persistence;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class CustomerCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomerCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_Customer_HappyPath()
    {
        var client = _factory.CreateClient();

        var createPayload = JsonSerializer.Serialize(new
        {
            name = "Demo User",
            email = $"demo-{Guid.NewGuid():N}@example.com",
            dateOfBirth = "1995-01-01"
        });

        var createResponse = await client.PostAsync("/api/customers", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        createResponse.IsSuccessStatusCode.Should().BeTrue();

        var createJson = await createResponse.Content.ReadAsStringAsync();
        using var createDoc = JsonDocument.Parse(createJson);
        var id = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            name = "Updated User",
            email = $"updated-{Guid.NewGuid():N}@example.com",
            dateOfBirth = "1994-12-31"
        });

        var updateResponse = await client.PutAsync($"/api/customers/{id}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/customers/{id}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var customer = await db.Customers.AsNoTracking().SingleAsync(c => c.Id == id);
        customer.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_Unknown_Customer_Should_Return_NotFound()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/customers/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}

