using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class RbacRolePermissionCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public RbacRolePermissionCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Deactivate_Role_With_Permission_HappyPath()
    {
        var client = _factory.CreateClient();

        var permissionPayload = JsonSerializer.Serialize(new
        {
            code = $"rbac.test.{Guid.NewGuid():N}".Substring(0, 16),
            name = "Test Permission",
            description = "Integration test permission"
        });

        var permissionResponse = await client.PostAsync("/api/permissions", new StringContent(permissionPayload, Encoding.UTF8, "application/json"));
        var permissionBody = await permissionResponse.Content.ReadAsStringAsync();
        permissionResponse.IsSuccessStatusCode.Should().BeTrue(permissionBody);

        using var permissionDoc = JsonDocument.Parse(permissionBody);
        var permissionId = permissionDoc.RootElement.GetProperty("id").GetGuid();

        var rolePayload = JsonSerializer.Serialize(new
        {
            name = $"TestRole-{Guid.NewGuid():N}".Substring(0, 12),
            description = "Integration test role",
            permissionIds = new[] { permissionId }
        });

        var roleResponse = await client.PostAsync("/api/roles", new StringContent(rolePayload, Encoding.UTF8, "application/json"));
        var roleBody = await roleResponse.Content.ReadAsStringAsync();
        roleResponse.IsSuccessStatusCode.Should().BeTrue(roleBody);

        using var roleDoc = JsonDocument.Parse(roleBody);
        var roleId = roleDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            name = $"TestRole-{Guid.NewGuid():N}".Substring(0, 12),
            description = "Updated",
            permissionIds = new[] { permissionId }
        });

        var updateResponse = await client.PutAsync($"/api/roles/{roleId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        var updateBody = await updateResponse.Content.ReadAsStringAsync();
        updateResponse.IsSuccessStatusCode.Should().BeTrue(updateBody);

        var deleteResponse = await client.DeleteAsync($"/api/roles/{roleId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var role = await db.Roles.AsNoTracking().SingleAsync(r => r.Id == roleId);
        role.IsActive.Should().BeFalse();
    }
}
