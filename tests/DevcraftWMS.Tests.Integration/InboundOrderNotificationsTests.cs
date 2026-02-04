using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class InboundOrderNotificationsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InboundOrderNotificationsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CompleteInboundOrder_Should_Create_Notifications()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        var orderId = await CreateInboundOrderAsync(client, warehouseId, productId, uomId);

        var startResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/receipts/start", new StringContent("{}", Encoding.UTF8, "application/json"));
        startResponse.IsSuccessStatusCode.Should().BeTrue(await startResponse.Content.ReadAsStringAsync());

        var completePayload = JsonSerializer.Serialize(new { allowPartial = true, notes = "Close OE with partial allowed" });
        var completeResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/complete", new StringContent(completePayload, Encoding.UTF8, "application/json"));
        completeResponse.IsSuccessStatusCode.Should().BeTrue(await completeResponse.Content.ReadAsStringAsync());

        var notificationsResponse = await client.GetAsync($"/api/inbound-orders/{orderId}/notifications");
        var notificationsBody = await notificationsResponse.Content.ReadAsStringAsync();
        notificationsResponse.IsSuccessStatusCode.Should().BeTrue(notificationsBody);

        using var document = JsonDocument.Parse(notificationsBody);
        document.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
        document.RootElement.GetArrayLength().Should().BeGreaterThan(0);
    }

    private static async Task<Guid> CreateInboundOrderAsync(HttpClient client, Guid warehouseId, Guid productId, Guid uomId)
    {
        var asnNumber = $"ASN-NOTIFY-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var asnPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber,
            documentNumber = "DOC-NOTIFY-001",
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
        submitResponse.IsSuccessStatusCode.Should().BeTrue(await submitResponse.Content.ReadAsStringAsync());

        var approveResponse = await client.PostAsync($"/api/asns/{asnId}/approve", new StringContent("{\"notes\":\"approve\"}", Encoding.UTF8, "application/json"));
        approveResponse.IsSuccessStatusCode.Should().BeTrue(await approveResponse.Content.ReadAsStringAsync());

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
            code = $"WH-NOTIFY-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
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
            code = $"SKU-NOTIFY-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
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
