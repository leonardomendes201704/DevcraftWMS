using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class WarehouseCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public WarehouseCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_Warehouse_HappyPath()
    {
        var client = _factory.CreateClient();

        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code,
            name = "Main Warehouse",
            shortName = "MAIN",
            description = "Primary DC",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-01",
            erpCode = "ERP-01",
            costCenterCode = "CC-01",
            costCenterName = "Ops",
            cutoffTime = "18:00:00",
            timezone = "America/Sao_Paulo",
            address = new
            {
                addressLine1 = "Rua Central, 100",
                addressLine2 = "Bloco A",
                district = "Centro",
                city = "Sao Paulo",
                state = "SP",
                postalCode = "01000-000",
                country = "BR",
                latitude = -23.55052m,
                longitude = -46.633308m
            },
            contact = new
            {
                contactName = "Ops Manager",
                contactEmail = "ops@example.com",
                contactPhone = "+55 11 99999-0000"
            },
            capacity = new
            {
                lengthMeters = 120.5m,
                widthMeters = 80.2m,
                heightMeters = 12.3m,
                totalAreaM2 = 9650.1m,
                totalCapacity = 1200m,
                capacityUnit = 3,
                maxWeightKg = 150000m,
                operationalArea = 8000m
            }
        });

        var createResponse = await client.PostAsync("/api/warehouses", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var id = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            code,
            name = "Main Warehouse Updated",
            shortName = "MAIN-01",
            description = "Primary DC - updated",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-01",
            erpCode = "ERP-01",
            costCenterCode = "CC-01",
            costCenterName = "Ops",
            cutoffTime = "19:00:00",
            timezone = "America/Sao_Paulo",
            address = new
            {
                addressLine1 = "Rua Central, 101",
                addressLine2 = "Bloco B",
                district = "Centro",
                city = "Sao Paulo",
                state = "SP",
                postalCode = "01000-000",
                country = "BR",
                latitude = -23.55052m,
                longitude = -46.633308m
            },
            contact = new
            {
                contactName = "Ops Manager",
                contactEmail = "ops@example.com",
                contactPhone = "+55 11 99999-0000"
            },
            capacity = new
            {
                lengthMeters = 121.5m,
                widthMeters = 81.2m,
                heightMeters = 12.3m,
                totalAreaM2 = 9800.1m,
                totalCapacity = 1250m,
                capacityUnit = 3,
                maxWeightKg = 150000m,
                operationalArea = 8200m
            }
        });

        var updateResponse = await client.PutAsync($"/api/warehouses/{id}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/warehouses/{id}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var warehouse = await db.Warehouses.AsNoTracking().SingleAsync(w => w.Id == id);
        warehouse.IsActive.Should().BeFalse();
        var addressCount = await db.WarehouseAddresses.CountAsync(a => a.WarehouseId == id);
        addressCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Delete_Unknown_Warehouse_Should_Return_NotFound()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/warehouses/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}
