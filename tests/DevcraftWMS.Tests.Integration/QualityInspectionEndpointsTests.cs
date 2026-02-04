using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class QualityInspectionEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public QualityInspectionEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Quarantined_Lot_Should_Create_Inspection_And_Approve_Releases()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var sectorId = await CreateSectorAsync(client, warehouseId);
        var sectionId = await CreateSectionAsync(client, sectorId);
        var structureId = await CreateStructureAsync(client, sectionId);
        var zoneId = await CreateZoneAsync(client, warehouseId, 4);
        var locationId = await CreateLocationAsync(client, structureId, zoneId);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId, trackingMode: 2, minimumShelfLifeDays: 30);

        var receiptId = await CreateReceiptAsync(client, warehouseId);
        var expirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        var addItemPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            lotCode = "LOT-QA-001",
            expirationDate,
            locationId,
            uomId,
            quantity = 5m,
            unitCost = 2.5m
        });

        var addItemResponse = await client.PostAsync($"/api/receipts/{receiptId}/items", new StringContent(addItemPayload, Encoding.UTF8, "application/json"));
        var addItemBody = await addItemResponse.Content.ReadAsStringAsync();
        addItemResponse.IsSuccessStatusCode.Should().BeTrue(addItemBody);

        var completeResponse = await client.PostAsync($"/api/receipts/{receiptId}/complete", new StringContent("{}", Encoding.UTF8, "application/json"));
        completeResponse.IsSuccessStatusCode.Should().BeTrue();

        var listResponse = await client.GetAsync("/api/quality-inspections?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);

        using var listDoc = JsonDocument.Parse(listBody);
        var inspectionElement = listDoc.RootElement.GetProperty("items")[0];
        var inspectionId = inspectionElement.GetProperty("id").GetGuid();
        var lotId = inspectionElement.GetProperty("lotId").GetGuid();

        var approveResponse = await client.PostAsync($"/api/quality-inspections/{inspectionId}/approve", new StringContent("{\"notes\":\"ok\"}", Encoding.UTF8, "application/json"));
        var approveBody = await approveResponse.Content.ReadAsStringAsync();
        approveResponse.IsSuccessStatusCode.Should().BeTrue(approveBody);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var lot = await db.Lots.AsNoTracking().SingleAsync(l => l.Id == lotId);
        lot.Status.Should().Be((Domain.Enums.LotStatus)0);

        var balance = await db.InventoryBalances.AsNoTracking().SingleOrDefaultAsync(b => b.LotId == lotId);
        balance.Should().NotBeNull();
        balance!.Status.Should().Be((Domain.Enums.InventoryBalanceStatus)0);
    }

    private static async Task<Guid> CreateReceiptAsync(HttpClient client, Guid warehouseId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            receiptNumber = "RCV-QA-001",
            documentNumber = "DOC-QA-001",
            supplierName = "Supplier QA",
            notes = "QA receipt"
        });

        var response = await client.PostAsync("/api/receipts", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateZoneAsync(HttpClient client, Guid warehouseId, int zoneType)
    {
        var code = $"ZON-QA-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Quarantine",
            description = "Quarantine zone",
            zoneType
        });

        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/zones", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Quality Warehouse",
            shortName = "QUAL",
            description = "QA",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-QA",
            erpCode = "ERP-QA",
            costCenterCode = "CC-QA",
            costCenterName = "QA",
            cutoffTime = "18:00:00",
            timezone = "America/Sao_Paulo",
            address = new
            {
                addressLine1 = "Rua QA, 100",
                addressLine2 = "Bloco A",
                district = "Centro",
                city = "Sao Paulo",
                state = "SP",
                postalCode = "01000-000",
                country = "BR",
                latitude = -23.55052m,
                longitude = -46.633308m
            }
        });

        var response = await client.PostAsync("/api/warehouses", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectorAsync(HttpClient client, Guid warehouseId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = "SEC-01",
            name = "QA Sector",
            description = "QA",
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
            code = "SEC-A",
            name = "QA Section",
            description = "QA"
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
            code = "STR-01",
            name = "QA Structure",
            structureType = 0,
            levels = 2
        });

        var response = await client.PostAsync($"/api/sections/{sectionId}/structures", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateLocationAsync(HttpClient client, Guid structureId, Guid zoneId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            structureId,
            zoneId,
            code = "Q-LOC-01",
            barcode = "Q-LOC-01",
            level = 1,
            row = 1,
            column = 1,
            maxWeightKg = 1000m,
            maxVolumeM3 = 2.5m,
            allowLotTracking = true,
            allowExpiryTracking = true
        });

        var response = await client.PostAsync($"/api/structures/{structureId}/locations", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client)
    {
        var code = $"UM{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Unit",
            type = 0
        });

        var response = await client.PostAsync("/api/uoms", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid baseUomId, int trackingMode, int minimumShelfLifeDays)
    {
        var code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "QA Product",
            description = "Quality product",
            ean = "789000000001",
            erpCode = "ERP-QA",
            category = "Quality",
            brand = "Devcraft",
            baseUomId,
            trackingMode,
            minimumShelfLifeDays,
            weightKg = 1.1,
            lengthCm = 10,
            widthCm = 5,
            heightCm = 3,
            volumeCm3 = 150
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
