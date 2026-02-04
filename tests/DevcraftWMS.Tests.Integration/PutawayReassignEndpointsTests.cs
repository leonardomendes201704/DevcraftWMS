using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class PutawayReassignEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PutawayReassignEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Reassign_Should_Record_History()
    {
        var client = _factory.CreateClient();

        var warehouseId = await CreateWarehouseAsync(client);
        var sectorId = await CreateSectorAsync(client, warehouseId);
        var sectionId = await CreateSectionAsync(client, sectorId);
        var structureId = await CreateStructureAsync(client, sectionId);
        var zoneId = await CreateZoneAsync(client, warehouseId);
        var locationId = await CreateLocationAsync(client, structureId, zoneId, "LOC-REASSIGN");

        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        var receiptId = await CreateReceiptAsync(client, warehouseId);

        var addItemPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            lotCode = (string?)null,
            expirationDate = (DateOnly?)null,
            locationId,
            uomId,
            quantity = 1m,
            unitCost = 3m
        });

        var addItemResponse = await client.PostAsync($"/api/receipts/{receiptId}/items", new StringContent(addItemPayload, Encoding.UTF8, "application/json"));
        addItemResponse.IsSuccessStatusCode.Should().BeTrue();

        var completeResponse = await client.PostAsync($"/api/receipts/{receiptId}/complete", new StringContent("{}", Encoding.UTF8, "application/json"));
        completeResponse.IsSuccessStatusCode.Should().BeTrue();

        var unitLoadResponse = await client.PostAsync("/api/unit-loads", new StringContent(JsonSerializer.Serialize(new
        {
            receiptId,
            ssccExternal = (string?)null,
            notes = "Reassign"
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

        var reassignPayload = JsonSerializer.Serialize(new
        {
            assigneeEmail = "admin@admin.com.br",
            reason = "Shift change"
        });

        var reassignResponse = await client.PostAsync($"/api/putaway-tasks/{taskId}/reassign", new StringContent(reassignPayload, Encoding.UTF8, "application/json"));
        var reassignBody = await reassignResponse.Content.ReadAsStringAsync();
        reassignResponse.IsSuccessStatusCode.Should().BeTrue(reassignBody);

        using var reassignDoc = JsonDocument.Parse(reassignBody);
        var history = reassignDoc.RootElement.GetProperty("assignmentHistory");
        history.GetArrayLength().Should().Be(1);
        history[0].GetProperty("reason").GetString().Should().Be("Shift change");
        history[0].GetProperty("toUserEmail").GetString().Should().Be("admin@admin.com.br");
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"WH-PA-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Putaway Assign Warehouse",
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
        var payload = JsonSerializer.Serialize(new { code = "SEC-PA", name = "Sector PA" });
        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectionAsync(HttpClient client, Guid sectorId)
    {
        var payload = JsonSerializer.Serialize(new { code = "SEC-PA-A", name = "Section PA" });
        var response = await client.PostAsync($"/api/sectors/{sectorId}/sections", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateStructureAsync(HttpClient client, Guid sectionId)
    {
        var payload = JsonSerializer.Serialize(new { code = "STR-PA", name = "Rack", structureType = 1, levels = 3 });
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
            code = $"ZON-PA-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(),
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
            code = $"SKU-PA-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Putaway Assign Product",
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
            receiptNumber = "RCV-PA-001",
            documentNumber = "DOC-PA-001",
            supplierName = "Supplier PA",
            notes = "PA receipt"
        });

        var response = await client.PostAsync("/api/receipts", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
