using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;

namespace DevcraftWMS.Tests.Integration;

public sealed class DashboardKpiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DashboardKpiTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ExpiringLotsKpi_Returns_Count()
    {
        var client = _factory.CreateClient();
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        await CreateLotAsync(client, productId, "LOT-KPI");

        var response = await client.GetAsync("/api/dashboard/expiring-lots?days=90");
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var total = doc.RootElement.GetProperty("totalLots").GetInt32();
        total.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task InboundKpis_Returns_Counts()
    {
        var client = _factory.CreateClient();
        var inboundOrderId = await SeedInboundOrderAsync(_factory);
        var checkinId = await CreateGateCheckinAsync(client, inboundOrderId);
        await AssignDockAsync(client, checkinId);

        var response = await client.GetAsync("/api/dashboard/inbound-kpis?days=7");
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var arrivals = doc.RootElement.GetProperty("arrivals").GetInt32();
        var dockAssigned = doc.RootElement.GetProperty("dockAssigned").GetInt32();

        arrivals.Should().BeGreaterThanOrEqualTo(1);
        dockAssigned.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task OutboundKpis_Returns_Counts()
    {
        var client = _factory.CreateClient();
        await SeedOutboundKpisAsync(_factory);

        var response = await client.GetAsync("/api/dashboard/outbound-kpis?days=7");
        response.IsSuccessStatusCode.Should().BeTrue();

        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var pickingCompleted = doc.RootElement.GetProperty("pickingCompleted").GetInt32();
        var checksCompleted = doc.RootElement.GetProperty("checksCompleted").GetInt32();
        var shipmentsCompleted = doc.RootElement.GetProperty("shipmentsCompleted").GetInt32();

        pickingCompleted.Should().BeGreaterThanOrEqualTo(1);
        checksCompleted.Should().BeGreaterThanOrEqualTo(1);
        shipmentsCompleted.Should().BeGreaterThanOrEqualTo(1);
    }

    private static async Task<Guid> CreateUomAsync(HttpClient client)
    {
        var code = $"KPI{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant();
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
            name = "KPI Product",
            description = "KPI product",
            ean = "789000001111",
            erpCode = "ERP-KPI",
            category = "Demo",
            brand = "Devcraft",
            baseUomId,
            trackingMode = 2,
            minimumShelfLifeDays = 30,
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

    private static async Task CreateLotAsync(HttpClient client, Guid productId, string code)
    {
        var manufactureDate = DateTime.UtcNow.Date.AddDays(-10).ToString("yyyy-MM-dd");
        var expirationDate = DateTime.UtcNow.Date.AddDays(30).ToString("yyyy-MM-dd");
        var payload = JsonSerializer.Serialize(new
        {
            code,
            manufactureDate,
            expirationDate,
            status = 0
        });

        var response = await client.PostAsync($"/api/products/{productId}/lots", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    private static async Task<Guid> CreateGateCheckinAsync(HttpClient client, Guid inboundOrderId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            inboundOrderId,
            documentNumber = (string?)null,
            vehiclePlate = "ABC1234",
            driverName = "KPI Driver",
            carrierName = "Devcraft Carrier",
            arrivalAtUtc = DateTime.UtcNow,
            notes = "KPI gate checkin"
        });

        var response = await client.PostAsync("/api/gate/checkins", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var doc = JsonDocument.Parse(body);
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task AssignDockAsync(HttpClient client, Guid checkinId)
    {
        var payload = JsonSerializer.Serialize(new
        {
            dockCode = "DCK-01"
        });

        var response = await client.PostAsync($"/api/gate/checkins/{checkinId}/assign-dock", new StringContent(payload, Encoding.UTF8, "application/json"));
        response.IsSuccessStatusCode.Should().BeTrue();
    }

    private static async Task<Guid> SeedInboundOrderAsync(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = $"WH-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "KPI Warehouse",
            WarehouseType = WarehouseType.Other,
            IsReceivingEnabled = true,
            IsPickingEnabled = true,
            IsShippingEnabled = true,
            IsReturnsEnabled = false
        };

        var asn = new Asn
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = warehouse.Id,
            AsnNumber = $"ASN-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            DocumentNumber = $"DOC-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            Status = AsnStatus.Registered,
            ExpectedArrivalDate = DateOnly.FromDateTime(DateTime.UtcNow.Date)
        };

        var inboundOrder = new InboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = warehouse.Id,
            AsnId = asn.Id,
            OrderNumber = $"OE-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(),
            DocumentNumber = asn.DocumentNumber,
            Status = InboundOrderStatus.Issued,
            Priority = InboundOrderPriority.Normal,
            InspectionLevel = InboundOrderInspectionLevel.None
        };

        db.Warehouses.Add(warehouse);
        db.Asns.Add(asn);
        db.InboundOrders.Add(inboundOrder);
        await db.SaveChangesAsync();

        return inboundOrder.Id;
    }

    private static async Task SeedOutboundKpisAsync(CustomWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = $"WH-OB-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Outbound KPI Warehouse",
            WarehouseType = WarehouseType.Other,
            IsReceivingEnabled = true,
            IsPickingEnabled = true,
            IsShippingEnabled = true,
            IsReturnsEnabled = false
        };

        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = warehouse.Id,
            OrderNumber = $"OS-{Guid.NewGuid():N}".Substring(0, 10).ToUpperInvariant(),
            Status = OutboundOrderStatus.Shipped,
            Priority = OutboundOrderPriority.Normal
        };

        var pickingTask = new PickingTask
        {
            Id = Guid.NewGuid(),
            OutboundOrderId = order.Id,
            WarehouseId = warehouse.Id,
            Status = PickingTaskStatus.Completed,
            CompletedAtUtc = DateTime.UtcNow
        };

        var check = new OutboundCheck
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OutboundOrderId = order.Id,
            WarehouseId = warehouse.Id,
            CheckedAtUtc = DateTime.UtcNow
        };

        var shipment = new OutboundShipment
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            OutboundOrderId = order.Id,
            WarehouseId = warehouse.Id,
            DockCode = "DCK-01",
            ShippedAtUtc = DateTime.UtcNow
        };

        db.Warehouses.Add(warehouse);
        db.OutboundOrders.Add(order);
        db.PickingTasks.Add(pickingTask);
        db.OutboundChecks.Add(check);
        db.OutboundShipments.Add(shipment);
        await db.SaveChangesAsync();
    }
}
