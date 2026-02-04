using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class InboundOrderCompletionTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InboundOrderCompletionTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CompleteInboundOrder_Should_Fail_When_Receipts_Not_Completed()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        var orderId = await CreateInboundOrderAsync(client, warehouseId, productId, uomId);

        var startResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/receipts/start", new StringContent("{}", Encoding.UTF8, "application/json"));
        var startBody = await startResponse.Content.ReadAsStringAsync();
        startResponse.IsSuccessStatusCode.Should().BeTrue(startBody);

        var completePayload = JsonSerializer.Serialize(new { allowPartial = false, notes = "Try to close" });
        var completeResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/complete", new StringContent(completePayload, Encoding.UTF8, "application/json"));
        completeResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CompleteInboundOrder_Should_Succeed_When_Receipts_And_Putaway_Completed()
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
        var orderId = await CreateInboundOrderAsync(client, warehouseId, productId, uomId);

        var startResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/receipts/start", new StringContent("{}", Encoding.UTF8, "application/json"));
        var startBody = await startResponse.Content.ReadAsStringAsync();
        startResponse.IsSuccessStatusCode.Should().BeTrue(startBody);

        using var startDoc = JsonDocument.Parse(startBody);
        var receiptId = startDoc.RootElement.GetProperty("id").GetGuid();

        var addItemPayload = JsonSerializer.Serialize(new
        {
            productId,
            lotId = (Guid?)null,
            lotCode = (string?)null,
            expirationDate = (DateOnly?)null,
            locationId = originLocationId,
            uomId,
            quantity = 5m,
            unitCost = 1.5m
        });

        var addItemResponse = await client.PostAsync($"/api/receipts/{receiptId}/items", new StringContent(addItemPayload, Encoding.UTF8, "application/json"));
        var addItemBody = await addItemResponse.Content.ReadAsStringAsync();
        addItemResponse.IsSuccessStatusCode.Should().BeTrue(addItemBody);

        var completeReceiptResponse = await client.PostAsync($"/api/receipts/{receiptId}/complete", new StringContent("{}", Encoding.UTF8, "application/json"));
        completeReceiptResponse.IsSuccessStatusCode.Should().BeTrue();

        var unitLoadResponse = await client.PostAsync("/api/unit-loads", new StringContent(JsonSerializer.Serialize(new
        {
            receiptId,
            ssccExternal = (string?)null,
            notes = "Putaway"
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

        var confirmPayload = JsonSerializer.Serialize(new { locationId = targetLocationId, notes = "Confirm" });
        var confirmResponse = await client.PostAsync($"/api/putaway-tasks/{taskId}/confirm", new StringContent(confirmPayload, Encoding.UTF8, "application/json"));
        var confirmBody = await confirmResponse.Content.ReadAsStringAsync();
        confirmResponse.IsSuccessStatusCode.Should().BeTrue(confirmBody);

        var closePayload = JsonSerializer.Serialize(new { allowPartial = false, notes = "Close OE" });
        var closeResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/complete", new StringContent(closePayload, Encoding.UTF8, "application/json"));
        var closeBody = await closeResponse.Content.ReadAsStringAsync();
        closeResponse.IsSuccessStatusCode.Should().BeTrue(closeBody);

        using var closeDoc = JsonDocument.Parse(closeBody);
        closeDoc.RootElement.GetProperty("status").GetInt32().Should().Be(3);
        closeDoc.RootElement.GetProperty("statusEvents").GetArrayLength().Should().BeGreaterThan(0);
    }

    private static async Task<Guid> CreateInboundOrderAsync(HttpClient client, Guid warehouseId, Guid productId, Guid uomId)
    {
        var asnNumber = $"ASN-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var asnPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber,
            documentNumber = "DOC-OE-001",
            supplierName = "Inbound Supplier",
            expectedArrivalDate = new DateOnly(2026, 2, 10),
            notes = "Inbound ASN"
        });

        var asnResponse = await client.PostAsync("/api/asns", new StringContent(asnPayload, Encoding.UTF8, "application/json"));
        var asnBody = await asnResponse.Content.ReadAsStringAsync();
        asnResponse.IsSuccessStatusCode.Should().BeTrue(asnBody);

        using var asnDoc = JsonDocument.Parse(asnBody);
        var asnId = asnDoc.RootElement.GetProperty("id").GetGuid();

        var itemPayload = JsonSerializer.Serialize(new
        {
            productId,
            uomId,
            quantity = 5m,
            lotCode = (string?)null,
            expirationDate = (DateOnly?)null
        });

        var addItemResponse = await client.PostAsync($"/api/asns/{asnId}/items", new StringContent(itemPayload, Encoding.UTF8, "application/json"));
        var addItemBody = await addItemResponse.Content.ReadAsStringAsync();
        addItemResponse.IsSuccessStatusCode.Should().BeTrue(addItemBody);

        var submitResponse = await client.PostAsync($"/api/asns/{asnId}/submit", new StringContent("{\"notes\":\"submit\"}", Encoding.UTF8, "application/json"));
        var submitBody = await submitResponse.Content.ReadAsStringAsync();
        submitResponse.IsSuccessStatusCode.Should().BeTrue(submitBody);

        var approveResponse = await client.PostAsync($"/api/asns/{asnId}/approve", new StringContent("{\"notes\":\"approve\"}", Encoding.UTF8, "application/json"));
        var approveBody = await approveResponse.Content.ReadAsStringAsync();
        approveResponse.IsSuccessStatusCode.Should().BeTrue(approveBody);

        var convertPayload = JsonSerializer.Serialize(new { asnId, notes = "Convert to OE" });
        var convertResponse = await client.PostAsync("/api/inbound-orders/from-asn", new StringContent(convertPayload, Encoding.UTF8, "application/json"));
        var convertBody = await convertResponse.Content.ReadAsStringAsync();
        convertResponse.IsSuccessStatusCode.Should().BeTrue(convertBody);

        using var orderDoc = JsonDocument.Parse(convertBody);
        return orderDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var payload = JsonSerializer.Serialize(new
        {
            code = $"WH-OE-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Inbound Warehouse",
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
        var payload = JsonSerializer.Serialize(new { code = "SEC-OE", name = "Inbound Sector" });
        var response = await client.PostAsync($"/api/warehouses/{warehouseId}/sectors", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateSectionAsync(HttpClient client, Guid sectorId)
    {
        var payload = JsonSerializer.Serialize(new { code = "SEC-OE-A", name = "Inbound Section" });
        var response = await client.PostAsync($"/api/sectors/{sectorId}/sections", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateStructureAsync(HttpClient client, Guid sectionId)
    {
        var payload = JsonSerializer.Serialize(new { code = "STR-OE", name = "Rack", structureType = 1, levels = 3 });
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
            code = $"ZON-OE-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(),
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
        var code = $"UOM-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new { code, name = "Each", description = "Each" });
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
            code = $"SKU-OE-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Inbound Product",
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
}
