using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PerfectApiTemplate.Infrastructure.Persistence;
using PerfectApiTemplate.Tests.Integration.Fixtures;

namespace PerfectApiTemplate.Tests.Integration;

public sealed class AuthEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_Login_ChangePassword_HappyPath()
    {
        var client = _factory.CreateClient();
        var email = "user1@example.com";

        var registerPayload = JsonSerializer.Serialize(new { email, password = "Pass@1234", fullName = "User One" });
        var registerResponse = await client.PostAsync("/api/auth/register", new StringContent(registerPayload, Encoding.UTF8, "application/json"));
        if (!registerResponse.IsSuccessStatusCode)
        {
            var body = await registerResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Register failed: {(int)registerResponse.StatusCode} {body}");
        }

        var loginPayload = JsonSerializer.Serialize(new { email, password = "Pass@1234" });
        var loginResponse = await client.PostAsync("/api/auth/login", new StringContent(loginPayload, Encoding.UTF8, "application/json"));
        loginResponse.IsSuccessStatusCode.Should().BeTrue();

        var loginJson = await loginResponse.Content.ReadAsStringAsync();
        using var loginDoc = JsonDocument.Parse(loginJson);
        var token = loginDoc.RootElement.GetProperty("token").GetString();
        token.Should().NotBeNullOrWhiteSpace();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var changePasswordPayload = JsonSerializer.Serialize(new { currentPassword = "Pass@1234" });
        var changeResponse = await client.PostAsync("/api/auth/change-password", new StringContent(changePasswordPayload, Encoding.UTF8, "application/json"));
        changeResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact]
    public async Task Login_With_Wrong_Password_Should_Fail_And_Not_Create_User()
    {
        var client = _factory.CreateClient();
        var email = "user2@example.com";

        var registerPayload = JsonSerializer.Serialize(new { email, password = "Pass@1234", fullName = "User Two" });
        var registerResponse = await client.PostAsync("/api/auth/register", new StringContent(registerPayload, Encoding.UTF8, "application/json"));
        if (!registerResponse.IsSuccessStatusCode)
        {
            var body = await registerResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Register failed: {(int)registerResponse.StatusCode} {body}");
        }

        var loginPayload = JsonSerializer.Serialize(new { email, password = "WrongPass" });
        var loginResponse = await client.PostAsync("/api/auth/login", new StringContent(loginPayload, Encoding.UTF8, "application/json"));
        loginResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.Users.CountAsync(u => u.Email == email);
        count.Should().Be(1);
    }

    [Fact]
    public async Task Register_Duplicate_Email_Should_Fail_And_Not_Create_Extra_User()
    {
        var client = _factory.CreateClient();
        var email = "user3@example.com";

        var payload = JsonSerializer.Serialize(new { email, password = "Pass@1234", fullName = "User Three" });
        var response1 = await client.PostAsync("/api/auth/register", new StringContent(payload, Encoding.UTF8, "application/json"));
        if (!response1.IsSuccessStatusCode)
        {
            var body = await response1.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Register failed: {(int)response1.StatusCode} {body}");
        }

        var response2 = await client.PostAsync("/api/auth/register", new StringContent(payload, Encoding.UTF8, "application/json"));
        response2.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.Users.CountAsync(u => u.Email == email);
        count.Should().Be(1);
    }
}
