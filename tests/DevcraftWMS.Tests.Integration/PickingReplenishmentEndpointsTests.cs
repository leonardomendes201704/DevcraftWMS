using System.Text;
using System.Text.Json;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using DevcraftWMS.Infrastructure.Persistence;
using DevcraftWMS.Tests.Integration.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevcraftWMS.Tests.Integration;

public sealed class PickingReplenishmentEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PickingReplenishmentEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Generate_Should_Create_Replenishment_Task()
    {
        var client = _factory.CreateClient();
        var warehouseId = await SeedPickingScenarioAsync(_factory);

        var payload = JsonSerializer.Serialize(new { warehouseId });
        var response = await client.PostAsync("/api/picking-replenishments/generate", new StringContent(payload, Encoding.UTF8, "application/json"));
        var body = await response.Content.ReadAsStringAsync();
        response.IsSuccessStatusCode.Should().BeTrue(body);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var tasks = await db.PickingReplenishmentTasks.Where(t => t.WarehouseId == warehouseId).ToListAsync();
        tasks.Should().NotBeEmpty();
        tasks.Should().OnlyContain(t => t.Status == PickingReplenishmentStatus.Pending);
    }

    private static async Task<Guid> SeedPickingScenarioAsync(CustomWebApplicationFactory factory)
    {
        var customerId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = $"WH-REP-{Guid.NewGuid():N}".Substring(0, 12).ToUpperInvariant(),
            Name = "Replenishment Warehouse",
            ShortName = "REP",
            Description = "Replenishment",
            WarehouseType = WarehouseType.DistributionCenter,
            IsPrimary = true,
            IsPickingEnabled = true,
            IsReceivingEnabled = true,
            IsShippingEnabled = true,
            IsReturnsEnabled = true,
            ExternalId = "EXT-REP",
            ErpCode = "ERP-REP",
            CostCenterCode = "CC-REP",
            CostCenterName = "Replenishment"
        };

        var sector = new Sector
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouse.Id,
            Code = $"SEC-REP-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Rep Sector"
        };
        sector.CustomerAccesses.Add(new SectorCustomer { Id = Guid.NewGuid(), SectorId = sector.Id, CustomerId = customerId });

        var section = new Section
        {
            Id = Guid.NewGuid(),
            SectorId = sector.Id,
            Code = $"SEC-REP-A-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Rep Section"
        };
        section.CustomerAccesses.Add(new SectionCustomer { Id = Guid.NewGuid(), SectionId = section.Id, CustomerId = customerId });

        var structure = new Structure
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            Code = $"STR-REP-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Rep Structure",
            Levels = 1
        };
        structure.CustomerAccesses.Add(new StructureCustomer { Id = Guid.NewGuid(), StructureId = structure.Id, CustomerId = customerId });

        var pickingZone = new Zone
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouse.Id,
            Code = "ZON-PICK",
            Name = "Picking",
            ZoneType = ZoneType.Picking
        };
        var storageZone = new Zone
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouse.Id,
            Code = "ZON-STO",
            Name = "Storage",
            ZoneType = ZoneType.Storage
        };

        var pickingLocation = new Location
        {
            Id = Guid.NewGuid(),
            StructureId = structure.Id,
            ZoneId = pickingZone.Id,
            Code = "PICK-01",
            Barcode = "PICK-01",
            Level = 1,
            Row = 1,
            Column = 1
        };
        pickingLocation.CustomerAccesses.Add(new LocationCustomer { Id = Guid.NewGuid(), LocationId = pickingLocation.Id, CustomerId = customerId });

        var storageLocation = new Location
        {
            Id = Guid.NewGuid(),
            StructureId = structure.Id,
            ZoneId = storageZone.Id,
            Code = "STO-01",
            Barcode = "STO-01",
            Level = 1,
            Row = 1,
            Column = 2
        };
        storageLocation.CustomerAccesses.Add(new LocationCustomer { Id = Guid.NewGuid(), LocationId = storageLocation.Id, CustomerId = customerId });

        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA-REP", Name = "Each", Type = UomType.Unit };
        var product = new Product
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Code = $"SKU-REP-{Guid.NewGuid():N}".Substring(0, 8).ToUpperInvariant(),
            Name = "Rep Product",
            Category = "Rep",
            BaseUomId = uom.Id,
            TrackingMode = TrackingMode.None
        };

        db.Warehouses.Add(warehouse);
        db.Sectors.Add(sector);
        db.Sections.Add(section);
        db.Structures.Add(structure);
        db.Zones.AddRange(pickingZone, storageZone);
        db.Locations.AddRange(pickingLocation, storageLocation);
        db.Uoms.Add(uom);
        db.Products.Add(product);

        db.InventoryBalances.Add(new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = pickingLocation.Id,
            ProductId = product.Id,
            LotId = null,
            QuantityOnHand = 2,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        });

        db.InventoryBalances.Add(new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = storageLocation.Id,
            ProductId = product.Id,
            LotId = null,
            QuantityOnHand = 50,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        });

        await db.SaveChangesAsync();
        return warehouse.Id;
    }
}
