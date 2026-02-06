using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Integration;

public sealed class InventoryCountEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InventoryCountEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Start_Complete_InventoryCount_HappyPath()
    {
        var client = _factory.CreateClient();
        var (locationId, warehouseId) = await CreateLocationAsync(client);
        var productId = await CreateProductAsync(client);

        var balancePayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            quantityOnHand = 5,
            quantityReserved = 0,
            status = InventoryBalanceStatus.Available
        });

        var balanceResponse = await client.PostAsync($"/api/locations/{locationId}/inventory", new StringContent(balancePayload, Encoding.UTF8, "application/json"));
        balanceResponse.IsSuccessStatusCode.Should().BeTrue();

        var createPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            locationId,
            zoneId = (Guid?)null,
            notes = "Cycle count"
        });

        var createResponse = await client.PostAsync("/api/inventory-counts", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var countId = createDoc.RootElement.GetProperty("id").GetGuid();

        var startResponse = await client.PostAsync($"/api/inventory-counts/{countId}/start", new StringContent("{}", Encoding.UTF8, "application/json"));
        startResponse.IsSuccessStatusCode.Should().BeTrue();

        var completePayload = JsonSerializer.Serialize(new
        {
            items = new[]
            {
                new { inventoryCountItemId = createDoc.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid(), quantityCounted = 4 }
            },
            notes = "done"
        });

        var completeResponse = await client.PostAsync($"/api/inventory-counts/{countId}/complete", new StringContent(completePayload, Encoding.UTF8, "application/json"));
        completeResponse.IsSuccessStatusCode.Should().BeTrue();
    }

    private static async Task<(Guid locationId, Guid warehouseId)> CreateLocationAsync(HttpClient client)
    {
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
            column = 1,
            maxWeightKg = 1000m,
            maxVolumeM3 = 2.5m,
            allowLotTracking = true,
            allowExpiryTracking = true
        });

        var createResponse = await client.PostAsync($"/api/structures/{structureId}/locations", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        return (createDoc.RootElement.GetProperty("id").GetGuid(), warehouseId);
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

    private static async Task<Guid> CreateProductAsync(HttpClient client)
    {
        var uomCode = $"UN-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
        var uomId = await CreateUomAsync(client, uomCode, "Unit", UomType.Unit);
        var payload = JsonSerializer.Serialize(new
        {
            code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Sample Product",
            baseUomId = uomId,
            trackingMode = 0,
            minimumShelfLifeDays = 1
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client, string code, string name, UomType type)
    {
        var payload = JsonSerializer.Serialize(new { code, name, type });
        var response = await client.PostAsync("/api/uoms", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
