using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Infrastructure.Persistence;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class EmailEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public EmailEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Enqueue_Email_HappyPath_Should_Create_Record()
    {
        var client = _factory.CreateClient();

        var payload = JsonSerializer.Serialize(new
        {
            from = "sender@test.local",
            to = "recipient@test.local",
            subject = "Hello",
            body = "Email body",
            isHtml = false
        });

        var response = await client.PostAsync("/api/emails", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.IsSuccessStatusCode.Should().BeTrue();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var id = doc.RootElement.GetProperty("id").GetGuid();
        id.Should().NotBe(Guid.Empty);

        var getResponse = await client.GetAsync($"/api/emails/{id}");
        getResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.EmailMessages.CountAsync(x => x.Id == id);
        count.Should().Be(1);
    }

    [Fact]
    public async Task Enqueue_Email_Invalid_Request_Should_Return_Validation_And_Not_Persist()
    {
        var client = _factory.CreateClient();

        var payload = JsonSerializer.Serialize(new
        {
            from = "sender@test.local",
            to = "not-an-email",
            subject = "",
            body = "",
            isHtml = false
        });

        var response = await client.PostAsync("/api/emails", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.EmailMessages.CountAsync();
        count.Should().Be(0);
    }
}

