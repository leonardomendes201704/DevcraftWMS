using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class DockScheduleEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DockScheduleEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Create_Should_Block_Overlapping_Slots()
    {
        var client = _factory.CreateClient();
        var warehouseId = await CreateWarehouseAsync(client);
        var start = DateTime.UtcNow.AddHours(1);
        var end = start.AddHours(2);

        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            dockCode = "D1",
            slotStartUtc = start,
            slotEndUtc = end,
            outboundOrderId = (Guid?)null,
            notes = "slot"
        });

        var response = await client.PostAsync("/api/dock-schedules", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.IsSuccessStatusCode.Should().BeTrue();

        var overlapPayload = JsonSerializer.Serialize(new
        {
            warehouseId,
            dockCode = "D1",
            slotStartUtc = start.AddMinutes(30),
            slotEndUtc = end.AddMinutes(30),
            outboundOrderId = (Guid?)null,
            notes = "overlap"
        });

        var overlapResponse = await client.PostAsync("/api/dock-schedules", new StringContent(overlapPayload, Encoding.UTF8, "application/json"));
        overlapResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
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
}
