using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Integration;

public sealed class ReturnEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ReturnEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Return_Should_Create_Return_Order()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var uomId = await CreateUomAsync(client, "EA", "Each", UomType.Unit);
        var productId = await CreateProductAsync(client, uomId);

        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            returnNumber = $"RET-{Guid.NewGuid():N}".Substring(0, 12),
            outboundOrderId = (Guid?)null,
            notes = "Return",
            items = new[]
            {
                new { productId, uomId, lotCode = (string?)null, expirationDate = (string?)null, quantityExpected = 2 }
            }
        });

        var response = await client.PostAsync("/api/returns", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
    }

    [Fact]
    public async Task Create_Return_Should_Fail_When_Items_Missing()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);

        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            returnNumber = $"RET-{Guid.NewGuid():N}".Substring(0, 12),
            items = Array.Empty<object>()
        });

        var response = await client.PostAsync("/api/returns", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var createPayload = JsonSerializer.Serialize(new
        {
            code,
            name = "Main Warehouse",
            shortName = "MAIN",
            description = "Primary DC",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-01",
            erpCode = "ERP-01",
            costCenterCode = "CC-01",
            costCenterName = "Ops",
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
            },
            contact = new
            {
                contactName = "Ops Manager",
                contactEmail = "ops@example.com",
                contactPhone = "+55 11 99999-0000"
            },
            capacity = new
            {
                lengthMeters = 120.5m,
                widthMeters = 80.2m,
                heightMeters = 12.3m,
                totalAreaM2 = 9650.1m,
                totalCapacity = 1200m,
                capacityUnit = 3,
                maxWeightKg = 150000m,
                operationalArea = 8000m
            }
        });

        var createResponse = await client.PostAsync("/api/warehouses", new StringContent(createPayload, Encoding.UTF8, "application/json"));
        var createBody = await createResponse.Content.ReadAsStringAsync();
        createResponse.IsSuccessStatusCode.Should().BeTrue(createBody);

        using var createDoc = JsonDocument.Parse(createBody);
        return createDoc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client, string code, string name, UomType type)
    {
        var payload = JsonSerializer.Serialize(new { code, name, type });
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
            code = $"SKU-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            name = "Sample Product",
            baseUomId = uomId,
            trackingMode = 0,
            minimumShelfLifeDays = 1
        });

        var response = await client.PostAsync("/api/products", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }
}
