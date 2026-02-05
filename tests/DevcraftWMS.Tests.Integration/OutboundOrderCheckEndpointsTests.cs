using System.Text;
using System.Text.Json;
using FluentAssertions;
using DevcraftWMS.Tests.Integration.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevcraftWMS.Tests.Integration;

public sealed class OutboundOrderCheckEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public OutboundOrderCheckEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Check_Should_Register_Divergence()
    {
        var client = _factory.CreateClient();

        var warehouseId = await CreateWarehouseAsync(client);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        await SeedInventoryBalanceAsync(_factory, warehouseId, productId, 10m);

        var orderId = await CreateOutboundOrderAsync(client, warehouseId, productId, uomId);
        await ReleaseOutboundOrderAsync(client, orderId);

        var orderResponse = await client.GetAsync($"/api/outbound-orders/{orderId}");
        var orderBody = await orderResponse.Content.ReadAsStringAsync();
        orderResponse.IsSuccessStatusCode.Should().BeTrue(orderBody);
        using var orderDoc = JsonDocument.Parse(orderBody);
        var orderItemId = orderDoc.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

        var checkPayload = JsonSerializer.Serialize(new
        {
            items = new[]
            {
                new
                {
                    outboundOrderItemId = orderItemId,
                    quantityChecked = 2m,
                    divergenceReason = "Short picked",
                    evidence = Array.Empty<object>()
                }
            },
            notes = "Checked"
        });

        var checkResponse = await client.PostAsync($"/api/outbound-orders/{orderId}/check", new StringContent(checkPayload, Encoding.UTF8, "application/json"));
        var checkBody = await checkResponse.Content.ReadAsStringAsync();
        checkResponse.IsSuccessStatusCode.Should().BeTrue(checkBody);

        using var checkDoc = JsonDocument.Parse(checkBody);
        checkDoc.RootElement.GetProperty("items").GetArrayLength().Should().Be(1);

        var refreshed = await client.GetAsync($"/api/outbound-orders/{orderId}");
        var refreshedBody = await refreshed.Content.ReadAsStringAsync();
        refreshed.IsSuccessStatusCode.Should().BeTrue(refreshedBody);
        using var refreshedDoc = JsonDocument.Parse(refreshedBody);
        refreshedDoc.RootElement.GetProperty("status").GetInt32().Should().Be(4);
    }

    [Fact]
    public async Task Check_Should_Accept_Evidence_File()
    {
        var client = _factory.CreateClient();

        var warehouseId = await CreateWarehouseAsync(client);
        var uomId = await CreateUomAsync(client);
        var productId = await CreateProductAsync(client, uomId);
        await SeedInventoryBalanceAsync(_factory, warehouseId, productId, 10m);

        var orderId = await CreateOutboundOrderAsync(client, warehouseId, productId, uomId);
        await ReleaseOutboundOrderAsync(client, orderId);

        var orderResponse = await client.GetAsync($"/api/outbound-orders/{orderId}");
        var orderBody = await orderResponse.Content.ReadAsStringAsync();
        orderResponse.IsSuccessStatusCode.Should().BeTrue(orderBody);
        using var orderDoc = JsonDocument.Parse(orderBody);
        var orderItemId = orderDoc.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();

        var evidenceContent = Convert.ToBase64String(new byte[] { 10, 20, 30 });
        var checkPayload = JsonSerializer.Serialize(new
        {
            items = new[]
            {
                new
                {
                    outboundOrderItemId = orderItemId,
                    quantityChecked = 3m,
                    divergenceReason = (string?)null,
                    evidence = new[]
                    {
                        new
                        {
                            fileName = "photo.jpg",
                            contentType = "image/jpeg",
                            sizeBytes = 3,
                            content = evidenceContent
                        }
                    }
                }
            },
            notes = "Checked with evidence"
        });

        var checkResponse = await client.PostAsync($"/api/outbound-orders/{orderId}/check", new StringContent(checkPayload, Encoding.UTF8, "application/json"));
        var checkBody = await checkResponse.Content.ReadAsStringAsync();
        checkResponse.IsSuccessStatusCode.Should().BeTrue(checkBody);

        using var checkDoc = JsonDocument.Parse(checkBody);
        checkDoc.RootElement.GetProperty("items")[0].GetProperty("evidenceCount").GetInt32().Should().Be(1);
    }

    private static async Task<Guid> CreateWarehouseAsync(HttpClient client)
    {
        var code = $"WH-CHK-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Check Warehouse",
            shortName = "CHK",
            description = "Outbound",
            warehouseType = 0,
            isPrimary = true,
            isPickingEnabled = true,
            isReceivingEnabled = true,
            isShippingEnabled = true,
            isReturnsEnabled = true,
            externalId = "EXT-CHK",
            erpCode = "ERP-CHK",
            costCenterCode = "CC-CHK",
            costCenterName = "Check",
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
        var code = $"SKU-CHK-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var ean = (DateTime.UtcNow.Ticks % 10000000000000L).ToString("D13");
        var payload = JsonSerializer.Serialize(new
        {
            code,
            name = "Check Product",
            description = "Outbound",
            ean,
            erpCode = $"ERP-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
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

    private static async Task<Guid> CreateOutboundOrderAsync(HttpClient client, Guid warehouseId, Guid productId, Guid uomId)
    {
        var orderNumber = $"OS-CHK-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant();
        var payload = JsonSerializer.Serialize(new
        {
            warehouseId,
            orderNumber,
            customerReference = "REF-CHK-100",
            carrierName = "Carrier",
            expectedShipDate = new DateOnly(2026, 2, 15),
            notes = "Outbound check",
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
        return doc.RootElement.GetProperty("id").GetGuid();
    }

    private static async Task ReleaseOutboundOrderAsync(HttpClient client, Guid orderId)
    {
        var releasePayload = JsonSerializer.Serialize(new
        {
            priority = 2,
            pickingMethod = 1,
            shippingWindowStartUtc = DateTime.UtcNow.AddHours(2),
            shippingWindowEndUtc = DateTime.UtcNow.AddHours(4)
        });

        var response = await client.PostAsync($"/api/outbound-orders/{orderId}/release", new StringContent(releasePayload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);
    }

    private static async Task SeedInventoryBalanceAsync(CustomWebApplicationFactory factory, Guid warehouseId, Guid productId, decimal quantity)
    {
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DevcraftWMS.Infrastructure.Persistence.ApplicationDbContext>();

        var sector = new DevcraftWMS.Domain.Entities.Sector
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = $"SEC-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Check Sector"
        };
        sector.CustomerAccesses.Add(new DevcraftWMS.Domain.Entities.SectorCustomer
        {
            Id = Guid.NewGuid(),
            SectorId = sector.Id,
            CustomerId = customerId
        });

        var section = new DevcraftWMS.Domain.Entities.Section
        {
            Id = Guid.NewGuid(),
            SectorId = sector.Id,
            Code = $"SEC-A-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Check Section"
        };
        section.CustomerAccesses.Add(new DevcraftWMS.Domain.Entities.SectionCustomer
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            CustomerId = customerId
        });

        var structure = new DevcraftWMS.Domain.Entities.Structure
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            Code = $"STR-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Check Structure",
            Levels = 1
        };
        structure.CustomerAccesses.Add(new DevcraftWMS.Domain.Entities.StructureCustomer
        {
            Id = Guid.NewGuid(),
            StructureId = structure.Id,
            CustomerId = customerId
        });

        var location = new DevcraftWMS.Domain.Entities.Location
        {
            Id = Guid.NewGuid(),
            StructureId = structure.Id,
            Code = $"LOC-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Barcode = $"LOC-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Level = 1,
            Row = 1,
            Column = 1
        };
        location.CustomerAccesses.Add(new DevcraftWMS.Domain.Entities.LocationCustomer
        {
            Id = Guid.NewGuid(),
            LocationId = location.Id,
            CustomerId = customerId
        });

        db.Sectors.Add(sector);
        db.Sections.Add(section);
        db.Structures.Add(structure);
        db.Locations.Add(location);
        await db.SaveChangesAsync();

        db.InventoryBalances.Add(new DevcraftWMS.Domain.Entities.InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = location.Id,
            ProductId = productId,
            LotId = null,
            QuantityOnHand = quantity,
            QuantityReserved = 0,
            Status = DevcraftWMS.Domain.Enums.InventoryBalanceStatus.Available
        });
        await db.SaveChangesAsync();
    }
}
