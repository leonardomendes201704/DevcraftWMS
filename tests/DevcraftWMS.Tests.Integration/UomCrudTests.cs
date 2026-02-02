using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Integration;

public sealed class UomCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public UomCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_Uom_HappyPath()
    {
        var client = _factory.CreateClient();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = $"U{Guid.NewGuid():N}".Substring(0, 6).ToUpperInvariant(),
            name = "Unit",
            type = UomType.Unit
        });

        var createResponse = await client.PostAsync("/api/uoms", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var uomId = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            code = $"U{Guid.NewGuid():N}".Substring(0, 6).ToUpperInvariant(),
            name = "Unit Updated",
            type = UomType.Unit
        });

        var updateResponse = await client.PutAsync($"/api/uoms/{uomId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/uoms/{uomId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var uom = await db.Uoms.AsNoTracking().SingleAsync(u => u.Id == uomId);
        uom.IsActive.Should().BeFalse();
    }
}
