using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class InboundOrderCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public InboundOrderCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ConvertAsn_To_InboundOrder_Should_Succeed()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);

        var asnPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber = "ASN-TEST-200",
            documentNumber = "DOC-200",
            supplierName = "Inbound Supplier",
            expectedArrivalDate = new DateOnly(2026, 2, 10),
            notes = "Test ASN"
        });

        var asnResponse = await client.PostAsync("/api/asns", new StringContent(asnPayload, Encoding.UTF8, "application/json"));
        var asnBody = await asnResponse.Content.ReadAsStringAsync();
        asnResponse.IsSuccessStatusCode.Should().BeTrue(asnBody);

        using var asnDoc = JsonDocument.Parse(asnBody);
        var asnId = asnDoc.RootElement.GetProperty("id").GetGuid();

        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);

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
        var orderId = orderDoc.RootElement.GetProperty("id").GetGuid();
        orderDoc.RootElement.GetProperty("status").GetInt32().Should().Be(0);

        var parametersPayload = JsonSerializer.Serialize(new { inspectionLevel = 2, priority = 3, suggestedDock = "D1" });
        var parametersResponse = await client.PutAsync($"/api/inbound-orders/{orderId}/parameters", new StringContent(parametersPayload, Encoding.UTF8, "application/json"));
        var parametersBody = await parametersResponse.Content.ReadAsStringAsync();
        parametersResponse.IsSuccessStatusCode.Should().BeTrue(parametersBody);

        var cancelPayload = JsonSerializer.Serialize(new { reason = "Canceled for test" });
        var cancelResponse = await client.PostAsync($"/api/inbound-orders/{orderId}/cancel", new StringContent(cancelPayload, Encoding.UTF8, "application/json"));
        var cancelBody = await cancelResponse.Content.ReadAsStringAsync();
        cancelResponse.IsSuccessStatusCode.Should().BeTrue(cancelBody);

        var listResponse = await client.GetAsync("/api/inbound-orders?pageNumber=1&pageSize=20&orderBy=CreatedAtUtc&orderDir=desc");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);
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
            externalId = "EXT-INB",
            erpCode = "ERP-INB",
            costCenterCode = "CC-INB",
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
            name = "Inbound Product",
            description = "Inbound",
            ean = "789000000222",
            erpCode = "ERP-INB",
            category = "Inbound",
            brand = "Devcraft",
            baseUomId,
            trackingMode = 0,
            minimumShelfLifeDays = (int?)null,
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
