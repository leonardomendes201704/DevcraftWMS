using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Infrastructure.Persistence.Logging;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class LoggingTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public LoggingTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Request_Log_Should_Be_Stored_In_LogsDb()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/customers");
        response.IsSuccessStatusCode.Should().BeTrue();

        var exists = await WaitForAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
            return await db.RequestLogs.AnyAsync(x => x.Path == "/api/customers");
        });

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Error_Log_Should_Be_Stored_In_LogsDb()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/api/test/throw");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.InternalServerError);

        var exists = await WaitForAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
            return await db.ErrorLogs.AnyAsync(x => x.Path == "/api/test/throw");
        });

        exists.Should().BeTrue();
    }

    [Fact]
    public async Task Transaction_Log_Should_Be_Stored_In_LogsDb()
    {
        var client = _factory.CreateClient();
        var payload = new
        {
            name = "Log Test",
            email = $"log-{Guid.NewGuid():N}@example.com",
            dateOfBirth = DateTime.UtcNow.AddYears(-25).ToString("yyyy-MM-dd")
        };

        var response = await client.PostAsJsonAsync("/api/customers", payload);
        response.IsSuccessStatusCode.Should().BeTrue();

        var exists = await WaitForAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
            return await db.TransactionLogs.AnyAsync(x => x.EntityName == "Customer" && x.Operation == "Insert");
        });

        exists.Should().BeTrue();
    }

    private static async Task<bool> WaitForAsync(Func<Task<bool>> predicate, int retries = 25, int delayMs = 200)
    {
        for (var i = 0; i < retries; i++)
        {
            if (await predicate())
            {
                return true;
            }

            await Task.Delay(delayMs);
        }

        return false;
    }
}
