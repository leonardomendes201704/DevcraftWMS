using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class CostCenterCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public CostCenterCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_CostCenter_HappyPath()
    {
        var client = _factory.CreateClient();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = $"CC-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            name = "Cost Center",
            description = "Operations"
        });

        var createResponse = await client.PostAsync("/api/cost-centers", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var costCenterId = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            code = $"CC-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            name = "Cost Center Updated",
            description = "Updated"
        });

        var updateResponse = await client.PutAsync($"/api/cost-centers/{costCenterId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/cost-centers/{costCenterId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var costCenter = await db.CostCenters.AsNoTracking().SingleAsync(c => c.Id == costCenterId);
        costCenter.IsActive.Should().BeFalse();
    }
}
