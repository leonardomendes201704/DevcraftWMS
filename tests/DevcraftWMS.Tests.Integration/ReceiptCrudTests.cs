using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class ReceiptCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ReceiptCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_AddItem_Complete_Receipt_Should_Create_InventoryBalance()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var sectorId = await CreateSectorAsync(client, warehouseId);
        var sectionId = await CreateSectionAsync(client, sectorId);
        var structureId = await CreateStructureAsync(client, sectionId);
        var locationId = await CreateLocationAsync(client, structureId);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);

        var receiptPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            receiptNumber = "RCV-TEST-001",
            documentNumber = "DOC-001",
            supplierName = "Inbound Supplier",
            notes = "Test receipt"
        });

        var receiptResponse = await client.PostAsync("/api/receipts", new StringContent(receiptPayload, Encoding.UTF8, "application/json"));
        var receiptBody = await receiptResponse.Content.ReadAsStringAsync();
        receiptResponse.IsSuccessStatusCode.Should().BeTrue(receiptBody);

        using var receiptDoc = JsonDocument.Parse(receiptBody);
        var receiptId = receiptDoc.RootElement.GetProperty("id").GetGuid();

        var itemPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            locationId,
            uomId,
            quantity = 10.5m,
            unitCost = 2.35m
        });

        var addItemResponse = await client.PostAsync($"/api/receipts/{receiptId}/items", new StringContent(itemPayload, Encoding.UTF8, "application/json"));
        var addItemBody = await addItemResponse.Content.ReadAsStringAsync();
        addItemResponse.IsSuccessStatusCode.Should().BeTrue(addItemBody);

        var completeResponse = await client.PostAsync($"/api/receipts/{receiptId}/complete", new StringContent("{}", Encoding.UTF8, "application/json"));
        completeResponse.IsSuccessStatusCode.Should().BeTrue();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var balance = await db.InventoryBalances
            .AsNoTracking()
            .SingleOrDefaultAsync(b => b.LocationId == locationId && b.ProductId == productId);

        balance.Should().NotBeNull();
        balance!.QuantityOnHand.Should().Be(10.5m);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Inbound Warehouse",
            shortName = "INB",
            description = "Inbound",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-REC",
            erpCode = "ERP-REC",
            costCenterCode = "CC-REC",
            costCenterName = "Inbound",
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
            name = "Inbound Sector",
            description = "Receiving",
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
            code = "SEC-01",
            name = "Inbound Section",
            description = "Receiving"
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
            name = "Inbound Structure",
            structureType = 0,
            levels = 2
        });

        var response = await client.PostAsync($"/api/sections/{sectionId}/structures", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateLocationAsync(HttpClient client, Guid structureId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = "LOC-01",
            barcode = "LOC-01",
            level = 1,
            row = 1,
            column = 1
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

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid baseUomId)
    {
        var code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Receipt Product",
            description = "Inbound",
            ean = "789000000123",
            erpCode = "ERP-REC",
            category = "Inbound",
            brand = "Devcraft",
            baseUomId,
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
