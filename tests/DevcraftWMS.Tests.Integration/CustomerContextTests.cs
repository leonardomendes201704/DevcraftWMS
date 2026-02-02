using System.Net;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class CustomerContextTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CustomerContextTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Requests_Without_Customer_Header_Return_BadRequest()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Remove("X-Customer-Id");

        var response = await client.GetAsync("/api/warehouses");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
