using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class OutboundOrderCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public OutboundOrderCrudTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_List_Get_Should_Succeed()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);

        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            orderNumber = "OS-TEST-100",
            customerReference = "REF-100",
            carrierName = "Carrier",
            expectedShipDate = new DateOnly(2026, 2, 15),
            notes = "Outbound test",
            items = new[]
            {
                new
                {
                    productId,
                    uomId,
                    quantity = 3m,
                    lotCode = (string?)null,
                    expirationDate = (DateOnly?)null
                }
            }
        });

        var response = await client.PostAsync("/api/outbound-orders", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        var orderId = doc.RootElement.GetProperty("id").GetGuid();

        var getResponse = await client.GetAsync($"/api/outbound-orders/{orderId}");
        var getBody = await getResponse.Content.ReadAsStringAsync();
        getResponse.IsSuccessStatusCode.Should().BeTrue(getBody);

        var releasePayload = JsonSerializer.Serialize(new
        {
            priority = 2,
            pickingMethod = 1,
            shippingWindowStartUtc = DateTime.UtcNow.AddHours(2),
            shippingWindowEndUtc = DateTime.UtcNow.AddHours(4)
        });

        var releaseResponse = await client.PostAsync($"/api/outbound-orders/{orderId}/release",
            new StringContent(releasePayload, Encoding.UTF8, "application/json"));
        var releaseBody = await releaseResponse.Content.ReadAsStringAsync();
        releaseResponse.IsSuccessStatusCode.Should().BeTrue(releaseBody);

        using var releasedDoc = JsonDocument.Parse(releaseBody);
        releasedDoc.RootElement.GetProperty("status").GetInt32().Should().Be(2);

        var listResponse = await client.GetAsync("/api/outbound-orders?pageNumber=1&pageSize=20&orderBy=CreatedAtUtc&orderDir=desc");
        var listBody = await listResponse.Content.ReadAsStringAsync();
        listResponse.IsSuccessStatusCode.Should().BeTrue(listBody);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Outbound Warehouse",
            shortName = "OUT",
            description = "Outbound",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-OUT",
            erpCode = "ERP-OUT",
            costCenterCode = "CC-OUT",
            costCenterName = "Outbound",
            cutoffTime = "18:00:00",
            timezone = "America/Sao_Paulo",
            address = new
            {
                addressLine1 = "Rua Central, 200",
                addressLine2 = "Bloco B",
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
            name = "Outbound Product",
            description = "Outbound",
            ean = "789000000333",
            erpCode = "ERP-OUT",
            category = "Outbound",
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
