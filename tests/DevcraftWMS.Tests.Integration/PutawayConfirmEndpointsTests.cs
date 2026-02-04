using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class PutawayConfirmEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PutawayConfirmEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Confirm_Should_Move_Balance_And_Complete_Task()
    {
        var client = _factory.CreateClient();

        var warehouseId = await CreateWarehouseAsync(client);
        var sectorId = await CreateSectorAsync(client, warehouseId);
        var sectionId = await CreateSectionAsync(client, sectorId);
        var structureId = await CreateStructureAsync(client, sectionId);
        var zoneId = await CreateZoneAsync(client, warehouseId);
        var originLocationId = await CreateLocationAsync(client, structureId, zoneId, "LOC-ORIGIN");
        var targetLocationId = await CreateLocationAsync(client, structureId, zoneId, "LOC-TARGET");

        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        var receiptId = await CreateReceiptAsync(client, warehouseId);

        var addItemPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            lotCode = (string?)null,
            expirationDate = (DateOnly?)null,
            locationId = originLocationId,
            uomId,
            quantity = 2m,
            unitCost = 5m
        });

        var addItemResponse = await client.PostAsync($"/api/receipts/{receiptId}/items", new StringContent(addItemPayload, Encoding.UTF8, "application/json"));
        addItemResponse.IsSuccessStatusCode.Should().BeTrue();

        var completeResponse = await client.PostAsync($"/api/receipts/{receiptId}/complete", new StringContent("{}", Encoding.UTF8, "application/json"));
        completeResponse.IsSuccessStatusCode.Should().BeTrue();

        var unitLoadResponse = await client.PostAsync("/api/unit-loads", new StringContent(JsonSerializer.Serialize(new
        {
            receiptId,
            ssccExternal = (string?)null,
            notes = "Putaway confirm"
        }), Encoding.UTF8, "application/json"));
        unitLoadResponse.IsSuccessStatusCode.Should().BeTrue();
        using var unitLoadDoc = JsonDocument.Parse(await unitLoadResponse.Content.ReadAsStringAsync());
        var unitLoadId = unitLoadDoc.RootElement.GetProperty("id").GetGuid();

        var printResponse = await client.PostAsync($"/api/unit-loads/{unitLoadId}/print", new StringContent("{}", Encoding.UTF8, "application/json"));
        printResponse.IsSuccessStatusCode.Should().BeTrue();

        var listResponse = await client.GetAsync($"/api/putaway-tasks?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc&receiptId={receiptId}&unitLoadId={unitLoadId}");
        listResponse.IsSuccessStatusCode.Should().BeTrue();
        using var listDoc = JsonDocument.Parse(await listResponse.Content.ReadAsStringAsync());
        var taskId = listDoc.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

        var confirmPayload = JsonSerializer.Serialize(new
        {
            locationId = targetLocationId,
            notes = "Confirmed"
        });

        var confirmResponse = await client.PostAsync($"/api/putaway-tasks/{taskId}/confirm", new StringContent(confirmPayload, Encoding.UTF8, "application/json"));
        var confirmBody = await confirmResponse.Content.ReadAsStringAsync();
        confirmResponse.IsSuccessStatusCode.Should().BeTrue(confirmBody);

        var originInventory = await client.GetAsync($"/api/locations/{originLocationId}/inventory?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc&productId={productId}");
        originInventory.IsSuccessStatusCode.Should().BeTrue();
        using var originDoc = JsonDocument.Parse(await originInventory.Content.ReadAsStringAsync());
        var originItem = originDoc.RootElement.GetProperty("items").EnumerateArray().First();
        originItem.GetProperty("quantityOnHand").GetDecimal().Should().Be(0m);

        var targetInventory = await client.GetAsync($"/api/locations/{targetLocationId}/inventory?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc&productId={productId}");
        targetInventory.IsSuccessStatusCode.Should().BeTrue();
        using var targetDoc = JsonDocument.Parse(await targetInventory.Content.ReadAsStringAsync());
        var targetItem = targetDoc.RootElement.GetProperty("items").EnumerateArray().First();
        targetItem.GetProperty("quantityOnHand").GetDecimal().Should().Be(2m);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"WH-PU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Putaway Warehouse",
            addressLine1 = "Main",
            city = "Test",
            state = "SP",
            postalCode = "01000-000",
            country = "BR",
            isReceivingEnabled = true,
            isPickingEnabled = true
        });

        var response = await client.PostAsync("/api/warehouses", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectorAsync(HttpClient client, Guid warehouseId)
    {
        var payload = JsonSerializer.Serialize(new { code = "SEC-PU", name = "Sector PU" });
        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectionAsync(HttpClient client, Guid sectorId)
    {
        var payload = JsonSerializer.Serialize(new { code = "SEC-PU-A", name = "Section PU" });
        var response = await client.PostAsync($"/api/sectors/{sectorId}/sections", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateStructureAsync(HttpClient client, Guid sectionId)
    {
        var payload = JsonSerializer.Serialize(new { code = "STR-PU", name = "Rack", structureType = 1, levels = 3 });
        var response = await client.PostAsync($"/api/sections/{sectionId}/structures", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateZoneAsync(HttpClient client, Guid warehouseId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"ZON-PU-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(),
            name = "Storage",
            description = "Zone",
            zoneType = 0
        });

        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/zones", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateLocationAsync(HttpClient client, Guid structureId, Guid zoneId, string code)
    {
        var payload = JsonSerializer.Serialize(new
        {
            zoneId,
            code,
            barcode = code,
            level = 1,
            row = 1,
            column = 1,
            maxWeightKg = 100,
            maxVolumeM3 = 100,
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
        var payload = JsonSerializer.Serialize(new { code = "EA", name = "Each", description = "Each" });
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
            code = $"SKU-PU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Putaway Product",
            baseUomId = uomId,
            trackingMode = 0,
            weightKg = 1,
            volumeCm3 = 1000
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateReceiptAsync(HttpClient client, Guid warehouseId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            receiptNumber = "RCV-PU-001",
            documentNumber = "DOC-PU-001",
            supplierName = "Supplier PU",
            notes = "PU receipt"
        });

        var response = await client.PostAsync("/api/receipts", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
