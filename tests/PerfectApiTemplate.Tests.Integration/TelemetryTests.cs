using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Infrastructure.Persistence.Logging;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class TelemetryTests : IClassFixture<CustomWebApplicationFactory>
{
    private const string InternalKeyHeader = "X-Internal-Telemetry-Key";
    private const string InternalKey = "test-telemetry-key";
    private readonly CustomWebApplicationFactory _factory;

    public TelemetryTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Client_Event_Should_Be_Stored_In_ErrorLogs()
    {
        var client = _factory.CreateClient();
        var payload = new
        {
            eventType = "ui_exception",
            severity = "error",
            clientApp = "PerfectApiTemplate.DemoMvc",
            clientEnv = "Testing",
            clientUrl = "/Customers",
            clientRoute = "Customers/Index",
            httpMethod = "GET",
            correlationId = Guid.NewGuid().ToString("N"),
            requestId = Guid.NewGuid().ToString("N"),
            apiRequestId = Guid.NewGuid().ToString("N"),
            apiMethod = "GET",
            apiPath = "/api/customers",
            apiStatusCode = 500,
            durationMs = 1500,
            message = "Client telemetry test event",
            exceptionType = "TestException",
            stackTrace = "stack",
            detailsJson = "{\"token\":\"secret\"}",
            userId = (string?)null,
            tenantId = (string?)null,
            userAgent = "TestAgent",
            tags = "test",
            occurredAtUtc = DateTimeOffset.UtcNow
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/telemetry/client-events");
        request.Headers.TryAddWithoutValidation(InternalKeyHeader, InternalKey);
        request.Content = JsonContent.Create(payload);

        var response = await client.SendAsync(request);
        response.IsSuccessStatusCode.Should().BeTrue();

        var exists = await WaitForAsync(async () =>
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<LogsDbContext>();
            return await db.ErrorLogs.AnyAsync(x => x.Source == "client" && x.EventType == "ui_exception");
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
