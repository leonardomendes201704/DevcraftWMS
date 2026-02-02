using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Integration;

public sealed class InventoryBalanceCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InventoryBalanceCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Update_Delete_InventoryBalance_HappyPath()
    {
        var client = _factory.CreateClient();
        var locationId = await CreateLocationAsync(client);
        var productId = await CreateProductAsync(client);
        var lotId = await CreateLotAsync(client, productId);

        var createPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId,
            quantityOnHand = 100,
            quantityReserved = 10,
            status = InventoryBalanceStatus.Available
        });

        var createResponse = await client.PostAsync($"/api/locations/{locationId}/inventory", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var balanceId = createDoc.RootElement.GetProperty("id").GetGuid();

        var updatePayload = JsonSerializer.Serialize(new
        {
            locationId,
            productId,
            lotId,
            quantityOnHand = 120,
            quantityReserved = 15,
            status = InventoryBalanceStatus.Blocked
        });

        var updateResponse = await client.PutAsync($"/api/inventory/balances/{balanceId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        updateResponse.IsSuccessStatusCode.Should().BeTrue();

        var deleteResponse = await client.DeleteAsync($"/api/inventory/balances/{balanceId}");
        deleteResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var balance = await db.InventoryBalances.AsNoTracking().SingleAsync(b => b.Id == balanceId);
        balance.IsActive.Should().BeFalse();
        balance.QuantityOnHand.Should().Be(120);
        balance.QuantityReserved.Should().Be(15);
    }

    [Fact]
    public async Task Create_Should_Return_BadRequest_When_Reserved_Greater_Than_OnHand()
    {
        var client = _factory.CreateClient();
        var locationId = await CreateLocationAsync(client);
        var productId = await CreateProductAsync(client);

        var payload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            quantityOnHand = 5,
            quantityReserved = 10,
            status = InventoryBalanceStatus.Available
        });

        var response = await client.PostAsync($"/api/locations/{locationId}/inventory", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    private static async Task<Guid> CreateLocationAsync(HttpClient client)
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
        return createDoc.RootElement.GetProperty("id").GetGuid();
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
            trackingMode = 2,
            minimumShelfLifeDays = 30
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client, string code, string name, UomType type)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name,
            type
        });

        var response = await client.PostAsync("/api/uoms", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateLotAsync(HttpClient client, Guid productId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"LOT-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            manufactureDate = "2025-01-01",
            expirationDate = "2026-01-01",
            status = LotStatus.Available
        });

        var response = await client.PostAsync($"/api/products/{productId}/lots", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
