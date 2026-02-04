using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class PutawaySuggestionEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PutawaySuggestionEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Suggestions_Should_Return_Compatible_Locations()
    {
        var client = _factory.CreateClient();

        var warehouseId = await CreateWarehouseAsync(client);
        var sectorId = await CreateSectorAsync(client, warehouseId);
        var sectionId = await CreateSectionAsync(client, sectorId);
        var structureId = await CreateStructureAsync(client, sectionId);
        var storageZoneId = await CreateZoneAsync(client, warehouseId, 0);
        var quarantineZoneId = await CreateZoneAsync(client, warehouseId, 4);
        var okLocationId = await CreateLocationAsync(client, structureId, storageZoneId, "LOC-OK");
        await CreateLocationAsync(client, structureId, quarantineZoneId, "LOC-QA");

        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId, trackingMode: 2);
        var receiptId = await CreateReceiptAsync(client, warehouseId);

        var addItemPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            lotCode = "LOT-PT-001",
            expirationDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(120)),
            locationId = okLocationId,
            uomId,
            quantity = 1m,
            unitCost = 1m
        });

        var addItemResponse = await client.PostAsync($"/api/receipts/{receiptId}/items", new StringContent(addItemPayload, Encoding.UTF8, "application/json"));
        addItemResponse.IsSuccessStatusCode.Should().BeTrue();

        var unitLoadResponse = await client.PostAsync("/api/unit-loads", new StringContent(JsonSerializer.Serialize(new
        {
            receiptId,
            ssccExternal = (string?)null,
            notes = "Putaway"
        }), Encoding.UTF8, "application/json"));
        unitLoadResponse.IsSuccessStatusCode.Should().BeTrue();
        var unitLoadBody = await unitLoadResponse.Content.ReadAsStringAsync();
        using var unitLoadDoc = JsonDocument.Parse(unitLoadBody);
        var unitLoadId = unitLoadDoc.RootElement.GetProperty("id").GetGuid();

        var printResponse = await client.PostAsync($"/api/unit-loads/{unitLoadId}/print", new StringContent("{}", Encoding.UTF8, "application/json"));
        printResponse.IsSuccessStatusCode.Should().BeTrue();

        var listResponse = await client.GetAsync($"/api/putaway-tasks?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc&receiptId={receiptId}&unitLoadId={unitLoadId}");
        listResponse.IsSuccessStatusCode.Should().BeTrue();
        var listBody = await listResponse.Content.ReadAsStringAsync();
        using var listDoc = JsonDocument.Parse(listBody);
        var taskId = listDoc.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

        var suggestionsResponse = await client.GetAsync($"/api/putaway-tasks/{taskId}/suggestions?limit=5");
        suggestionsResponse.IsSuccessStatusCode.Should().BeTrue();
        var suggestionsBody = await suggestionsResponse.Content.ReadAsStringAsync();
        using var suggestionsDoc = JsonDocument.Parse(suggestionsBody);
        var first = suggestionsDoc.RootElement[0];
        first.GetProperty("locationCode").GetString().Should().Be("LOC-OK");
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"WH-PT-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
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
        var payload = JsonSerializer.Serialize(new { code = "SEC-PT", name = "Sector PT" });
        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectionAsync(HttpClient client, Guid sectorId)
    {
        var payload = JsonSerializer.Serialize(new { code = "SEC-A", name = "Section A" });
        var response = await client.PostAsync($"/api/sectors/{sectorId}/sections", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateStructureAsync(HttpClient client, Guid sectionId)
    {
        var payload = JsonSerializer.Serialize(new { code = "STR-PT", name = "Rack", structureType = 1, levels = 3 });
        var response = await client.PostAsync($"/api/sections/{sectionId}/structures", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateZoneAsync(HttpClient client, Guid warehouseId, int zoneType)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"ZON-PT-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(),
            name = zoneType == 4 ? "Quarantine" : "Storage",
            description = "Zone",
            zoneType
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

    private static async Task<Guid> CreateProductAsync(HttpClient client, Guid uomId, int trackingMode)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"SKU-PT-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Putaway Product",
            baseUomId = uomId,
            trackingMode,
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
            receiptNumber = "RCV-PT-001",
            documentNumber = "DOC-PT-001",
            supplierName = "Supplier PT",
            notes = "PT receipt"
        });

        var response = await client.PostAsync("/api/receipts", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
