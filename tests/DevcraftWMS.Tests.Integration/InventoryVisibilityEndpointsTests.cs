using System.Text;
using System.Text.Json;
using DevcraftWMS.Domain.Enums;
using DevcraftWMS.Tests.Integration.Fixtures;
using FluentAssertions;

namespace DevcraftWMS.Tests.Integration;

public sealed class InventoryVisibilityEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InventoryVisibilityEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_Inventory_Visibility_Should_Return_Summary_And_Locations()
    {
        var client = _factory.CreateClient();
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var warehouseId = await CreateWarehouseAsync(client);
        var locationId = await CreateLocationAsync(client, warehouseId);
        var uomId = await CreateUomAsync(client, $"EA-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(), "Each", UomType.Unit);
        var productId = await CreateProductAsync(client, uomId);
        var lotId = await CreateLotAsync(client, productId);

        var balancePayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId,
            quantityOnHand = 10,
            quantityReserved = 2,
            status = InventoryBalanceStatus.Available
        });

        var balanceResponse = await client.PostAsync($"/api/locations/{locationId}/inventory", new StringContent(balancePayload, Encoding.UTF8, "application/json"));
        var balanceBody = await balanceResponse.Content.ReadAsStringAsync();
        balanceResponse.IsSuccessStatusCode.Should().BeTrue(balanceBody);

        var response = await client.GetAsync($"/api/inventory-visibility?customerId={customerId}&warehouseId={warehouseId}");
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        var summary = doc.RootElement.GetProperty("summary").GetProperty("items");
        var locations = doc.RootElement.GetProperty("locations").GetProperty("items");

        summary.GetArrayLength().Should().Be(1);
        locations.GetArrayLength().Should().Be(1);

        summary[0].GetProperty("quantityAvailable").GetDecimal().Should().Be(8);
        summary[0].GetProperty("quantityOnHand").GetDecimal().Should().Be(10);
        summary[0].GetProperty("quantityReserved").GetDecimal().Should().Be(2);
    }

    [Fact]
    public async Task Export_Inventory_Visibility_Should_Return_Print_View()
    {
        var client = _factory.CreateClient();
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var warehouseId = await CreateWarehouseAsync(client);
        var locationId = await CreateLocationAsync(client, warehouseId);
        var uomId = await CreateUomAsync(client, $"EA-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(), "Each", UomType.Unit);
        var productId = await CreateProductAsync(client, uomId);

        var balancePayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            quantityOnHand = 5,
            quantityReserved = 0,
            status = InventoryBalanceStatus.Available
        });

        var balanceResponse = await client.PostAsync($"/api/locations/{locationId}/inventory", new StringContent(balancePayload, Encoding.UTF8, "application/json"));
        var balanceBody = await balanceResponse.Content.ReadAsStringAsync();
        balanceResponse.IsSuccessStatusCode.Should().BeTrue(balanceBody);

        var response = await client.GetAsync($"/api/inventory-visibility/export?format=print&customerId={customerId}&warehouseId={warehouseId}");
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        response.Content.Headers.ContentType?.MediaType.Should().Be("text/html");
        body.Should().Contain("Inventory Visibility");
    }

    [Fact]
    public async Task Get_Inventory_Visibility_Timeline_Should_Return_Movements()
    {
        var client = _factory.CreateClient();
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var warehouseId = await CreateWarehouseAsync(client);
        var fromLocationId = await CreateLocationAsync(client, warehouseId);
        var toLocationId = await CreateLocationAsync(client, warehouseId);
        var uomId = await CreateUomAsync(client, $"EA-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(), "Each", UomType.Unit);
        var productId = await CreateProductAsync(client, uomId);

        var balancePayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            quantityOnHand = 10,
            quantityReserved = 0,
            status = InventoryBalanceStatus.Available
        });

        var balanceResponse = await client.PostAsync($"/api/locations/{fromLocationId}/inventory", new StringContent(balancePayload, Encoding.UTF8, "application/json"));
        var balanceBody = await balanceResponse.Content.ReadAsStringAsync();
        balanceResponse.IsSuccessStatusCode.Should().BeTrue(balanceBody);

        var movementPayload = JsonSerializer.Serialize(new
        {
            fromLocationId,
            toLocationId,
            productId,
            lotId = (Guid?)null,
            quantity = 2,
            reason = "Timeline test",
            reference = "REF-TEST",
            performedAtUtc = DateTime.UtcNow
        });

        var movementResponse = await client.PostAsync("/api/inventory/movements", new StringContent(movementPayload, Encoding.UTF8, "application/json"));
        var movementBody = await movementResponse.Content.ReadAsStringAsync();
        movementResponse.IsSuccessStatusCode.Should().BeTrue(movementBody);

        var response = await client.GetAsync($"/api/inventory-visibility/{productId}/timeline?customerId={customerId}&warehouseId={warehouseId}&locationId={fromLocationId}");
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        doc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
        doc.RootElement.GetArrayLength().Should().BeGreaterThan(0);
        doc.RootElement[0].GetProperty("eventType").GetString().Should().Be("movement");
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

    private static async Task<Guid> CreateLocationAsync(HttpClient client, Guid warehouseId)
    {
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
        return createDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectorAsync(HttpClient client, Guid warehouseId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            code = $"SEC-{Guid.NewGuid():N}".Substring(0, 6).ToUpperInvariant(),
            name = "Sector",
            sectorType = 0
        });

        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectionAsync(HttpClient client, Guid sectorId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            sectorId,
            code = $"SECT-{Guid.NewGuid():N}".Substring(0, 6).ToUpperInvariant(),
            name = "Section"
        });

        var response = await client.PostAsync($"/api/sectors/{sectorId}/sections", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateStructureAsync(HttpClient client, Guid sectionId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            sectionId,
            code = $"STR-{Guid.NewGuid():N}".Substring(0, 6).ToUpperInvariant(),
            name = "Structure",
            structureType = 0,
            levels = 3
        });

        var response = await client.PostAsync($"/api/sections/{sectionId}/structures", new StringContent(payload, Encoding.UTF8, "application/json"));
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

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid uomId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Sample Product",
            baseUomId = uomId,
            trackingMode = 1,
            minimumShelfLifeDays = 1
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateLotAsync(HttpClient client, Guid productId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            productId,
            code = $"LOT-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            manufactureDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-10)),
            expirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(90)),
            status = LotStatus.Available
        });

        var response = await client.PostAsync($"/api/products/{productId}/lots", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
