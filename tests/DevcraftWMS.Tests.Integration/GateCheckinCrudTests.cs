using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class GateCheckinCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public GateCheckinCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GateCheckin_CRUD_Should_Succeed()
    {
        var client = _factory.CreateClient();
        var (orderId, documentNumber) = await CreateInboundOrderAsync(client);

        var createPayload = JsonSerializer.Serialize(new
        {
            inboundOrderId = orderId,
            documentNumber,
            vehiclePlate = "ABC1D23",
            driverName = "Driver Test",
            carrierName = "Carrier One",
            arrivalAtUtc = new DateTime(2026, 2, 3, 8, 0, 0, DateTimeKind.Utc),
            notes = "Initial check-in"
        });

        var createResponse = await client.PostAsync("/api/gate/checkins", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        var checkinId = createDoc.RootElement.GetProperty("id").GetGuid();
        createDoc.RootElement.GetProperty("status").GetInt32().Should().Be(0);

        var getResponse = await client.GetAsync($"/api/gate/checkins/{checkinId}");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        getResponse.IsSuccessStatusCode.Should().BeTrue(getBody);

        var updatePayload = JsonSerializer.Serialize(new
        {
            inboundOrderId = orderId,
            documentNumber,
            vehiclePlate = "XYZ9Z99",
            driverName = "Driver Updated",
            carrierName = "Carrier Updated",
            arrivalAtUtc = new DateTime(2026, 2, 3, 9, 0, 0, DateTimeKind.Utc),
            status = 1,
            notes = "Updated check-in"
        });

        var updateResponse = await client.PutAsync($"/api/gate/checkins/{checkinId}", new StringContent(updatePayload, Encoding.UTF8, "application/json"));
        var updateBody = await updateResponse.Content.ReadAsStringAsync();
        updateResponse.IsSuccessStatusCode.Should().BeTrue(updateBody);

        var listResponse = await client.GetAsync("/api/gate/checkins?pageNumber=1&pageSize=20&orderBy=ArrivalAtUtc&orderDir=desc&vehiclePlate=XYZ");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);

        var deleteResponse = await client.DeleteAsync($"/api/gate/checkins/{checkinId}");
        var deleteBody = await deleteResponse.Content.ReadAsStringAsync();
        deleteResponse.IsSuccessStatusCode.Should().BeTrue(deleteBody);
    }

    [Fact]
    public async Task Create_Should_Return_NotFound_When_InboundOrder_Missing()
    {
        var client = _factory.CreateClient();

        var payload = JsonSerializer.Serialize(new
        {
            inboundOrderId = Guid.NewGuid(),
            documentNumber = "DOC-404",
            vehiclePlate = "AAA1B23",
            driverName = "Driver Missing",
            carrierName = "Carrier Missing"
        });

        var response = await client.PostAsync("/api/gate/checkins", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<(Guid OrderId, string DocumentNumber)> CreateInboundOrderAsync(HttpClient client)
    {
        var warehouseId = await CreateWarehouseAsync(client);

        var documentNumber = "DOC-GATE-200";
        var asnPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            asnNumber = $"ASN-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            documentNumber,
            supplierName = "Inbound Supplier",
            expectedArrivalDate = new DateOnly(2026, 2, 10),
            notes = "Gate ASN"
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

        return (orderId, documentNumber);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Gate Warehouse",
            shortName = "GATE",
            description = "Inbound",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-GATE",
            erpCode = "ERP-GATE",
            costCenterCode = "CC-GATE",
            costCenterName = "Gate",
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
            name = "Gate Product",
            description = "Gate",
            ean = "789000000333",
            erpCode = "ERP-GATE",
            category = "Gate",
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
