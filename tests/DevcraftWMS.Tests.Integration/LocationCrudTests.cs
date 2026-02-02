using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class LocationCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public LocationCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_Location_HappyPath()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var sectorId = await CreateSectorAsync(client, warehouseId);
        var sectionId = await CreateSectionAsync(client, sectorId);
        var structureId = await CreateStructureAsync(client, sectionId);

        var code = $"L-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code,
            barcode = "BC-0001",
            level = 1,
            row = 1,
            column = 1
        });

        var createResponse = await client.PostAsync($"/api/structures/{structureId}/locations", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var locationId = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            structureId,
            code,
            barcode = "BC-0002",
            level = 2,
            row = 2,
            column = 2
        });

        var updateResponse = await client.PutAsync($"/api/locations/{locationId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/locations/{locationId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var location = await db.Locations.AsNoTracking().SingleAsync(l => l.Id == locationId);
        location.IsActive.Should().BeFalse();
        location.StructureId.Should().Be(structureId);
    }

    [Fact]
    public async Task Delete_Unknown_Location_Should_Return_NotFound()
    {
        var client = _factory.CreateClient();
        var response = await client.DeleteAsync($"/api/locations/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
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
        return createDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectorAsync(HttpClient client, Guid warehouseId)
    {
        var sectorCode = $"SEC-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = sectorCode,
            name = "Receiving",
            description = "Inbound",
            sectorType = 0
        });

        var createResponse = await client.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        return createDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectionAsync(HttpClient client, Guid sectorId)
    {
        var sectionCode = $"SEC-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = sectionCode,
            name = "Zone A",
            description = "Primary"
        });

        var createResponse = await client.PostAsync($"/api/sectors/{sectorId}/sections", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        return createDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateStructureAsync(HttpClient client, Guid sectionId)
    {
        var structureCode = $"R-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code = structureCode,
            name = "Rack A",
            structureType = 0,
            levels = 4
        });

        var createResponse = await client.PostAsync($"/api/sections/{sectionId}/structures", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        return createDoc.RootElement.GetProperty("id").GetGuid();
    }
}
