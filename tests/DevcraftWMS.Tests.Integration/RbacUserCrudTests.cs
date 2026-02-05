using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class RbacUserCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RbacUserCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Deactivate_User_HappyPath()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var role = await db.Roles.AsNoTracking().FirstAsync(r => r.Name == "Backoffice");

        var client = _factory.CreateClient();

        var createPayload = JsonSerializer.Serialize(new
        {
            email = $"user{Guid.NewGuid():N}@test.local",
            fullName = "Test User",
            password = "ChangeMe123!",
            roleIds = new[] { role.Id }
        });

        var createResponse = await client.PostAsync("/api/users", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var userId = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            email = $"user{Guid.NewGuid():N}@test.local",
            fullName = "Updated User",
            roleIds = new[] { role.Id }
        });

        var updateResponse = await client.PutAsync($"/api/users/{userId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        var updateBody = await updateResponse.Content.ReadAsStringAsync();
        updateResponse.IsSuccessStatusCode.Should().BeTrue(updateBody);

        var deleteResponse = await client.DeleteAsync($"/api/users/{userId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        var user = await db.Users.AsNoTracking().SingleAsync(u => u.Id == userId);
        user.IsActive.Should().BeFalse();
    }
}
