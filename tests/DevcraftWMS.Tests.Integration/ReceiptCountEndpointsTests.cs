using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class ReceiptCountEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ReceiptCountEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterCount_Should_Create_And_List_Counts()
    {
        var client = _factory.CreateClient();
        var inboundOrderId = await CreateInboundOrderAsync(client);

        var startResponse = await client.PostAsync($"/api/inbound-orders/{inboundOrderId}/receipts/start", new StringContent("{}", Encoding.UTF8, "application/json"));
        var startBody = await startResponse.Content.ReadAsStringAsync();
        startResponse.IsSuccessStatusCode.Should().BeTrue(startBody);

        using var startDoc = JsonDocument.Parse(startBody);
        var receiptId = startDoc.RootElement.GetProperty("id").GetGuid();

        var expectedResponse = await client.GetAsync($"/api/receipts/{receiptId}/expected-items");
        var expectedBody = await expectedResponse.Content.ReadAsStringAsync();
        expectedResponse.IsSuccessStatusCode.Should().BeTrue(expectedBody);

        using var expectedDoc = JsonDocument.Parse(expectedBody);
        var expectedItem = expectedDoc.RootElement.EnumerateArray().First();
        var inboundOrderItemId = expectedItem.GetProperty("inboundOrderItemId").GetGuid();
        var expectedQuantity = expectedItem.GetProperty("expectedQuantity").GetDecimal();

        var countPayload = JsonSerializer.Serialize(new
        {
            inboundOrderItemId,
            countedQuantity = expectedQuantity + 2,
            mode = 1,
            notes = "integration count"
        });

        var countResponse = await client.PostAsync($"/api/receipts/{receiptId}/counts", new StringContent(countPayload, Encoding.UTF8, "application/json"));
        var countBody = await countResponse.Content.ReadAsStringAsync();
        countResponse.IsSuccessStatusCode.Should().BeTrue(countBody);

        var listResponse = await client.GetAsync($"/api/receipts/{receiptId}/counts");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);

        using var listDoc = JsonDocument.Parse(listBody);
        var countItem = listDoc.RootElement.EnumerateArray().First();
        countItem.GetProperty("variance").GetDecimal().Should().Be(2m);
    }

    private static async Task<Guid> CreateInboundOrderAsync(HttpClient client)
    {
        var warehouseId = await CreateWarehouseAsync(client);

        var asnNumber = $"ASN-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var documentNumber = "DOC-REC-210";
        var asnPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber,
            documentNumber,
            supplierName = "Inbound Supplier",
            expectedArrivalDate = new DateOnly(2026, 2, 10),
            notes = "Receipt ASN"
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
        return orderDoc.RootElement.GetProperty("id").GetGuid();
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
        var ean = $"789{Random.Shared.Next(1000000000, 1999999999)}";
        var erpCode = $"ERP-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Receipt Product",
            description = "Inbound",
            ean,
            erpCode,
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
