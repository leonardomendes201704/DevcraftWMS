using FluentAssertions;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Health_Should_Return_Ok()
    {
        var response = await _client.GetAsync("/health");

        response.IsSuccessStatusCode.Should().BeTrue();
    }
}

