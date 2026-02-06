using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using DevcraftWMS.Infrastructure.Persistence;

namespace DevcraftWMS.Infrastructure.Seeding;

public sealed class SampleDataSeeder
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SampleDataOptions _options;
    private readonly ILogger<SampleDataSeeder> _logger;

    public SampleDataSeeder(
        ApplicationDbContext dbContext,
        IOptions<SampleDataOptions> options,
        ILogger<SampleDataSeeder> logger)
    {
        _dbContext = dbContext;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled)
        {
            return;
        }

        if (_options.ResetSeedData)
        {
            await CleanupSeedDataAsync(cancellationToken);
        }

        var normalizedWarehouseCode = _options.WarehouseCode.Trim().ToUpperInvariant();
        var customer = await EnsureCustomerAsync(cancellationToken);
        var uoms = await EnsureUomsAsync(cancellationToken);

        var warehouse = await EnsureWarehouseAsync(normalizedWarehouseCode, cancellationToken);
        var sector = await EnsureSectorAsync(warehouse.Id, cancellationToken);
        var section = await EnsureSectionAsync(sector.Id, cancellationToken);
        var structure = await EnsureStructureAsync(section.Id, cancellationToken);
        var aisle = await EnsureAisleAsync(section.Id, cancellationToken);
        var zone = await EnsureZoneAsync(warehouse.Id, cancellationToken);
        var quarantineZone = await EnsureQuarantineZoneAsync(warehouse.Id, cancellationToken);
        var crossDockZone = await EnsureCrossDockZoneAsync(warehouse.Id, cancellationToken);
        var locations = await EnsureLocationsAsync(structure.Id, zone.Id, cancellationToken);
        var quarantineLocations = await EnsureQuarantineLocationsAsync(structure.Id, quarantineZone.Id, cancellationToken);
        var crossDockLocations = await EnsureCrossDockLocationsAsync(structure.Id, crossDockZone.Id, cancellationToken);

        await EnsureSectorAccessAsync(sector, customer.Id, cancellationToken);
        await EnsureSectionAccessAsync(section, customer.Id, cancellationToken);
        await EnsureStructureAccessAsync(structure, customer.Id, cancellationToken);
        await EnsureAisleAccessAsync(aisle, customer.Id, cancellationToken);
        await EnsureZoneAccessAsync(zone, customer.Id, cancellationToken);
        await EnsureZoneAccessAsync(quarantineZone, customer.Id, cancellationToken);
        await EnsureZoneAccessAsync(crossDockZone, customer.Id, cancellationToken);
        await EnsureLocationAccessAsync(locations, customer.Id, cancellationToken);
        await EnsureLocationAccessAsync(quarantineLocations, customer.Id, cancellationToken);
        await EnsureLocationAccessAsync(crossDockLocations, customer.Id, cancellationToken);

        await EnsureProductsAsync(customer.Id, uoms.BaseUom.Id, uoms.BoxUom.Id, _options.ProductCount, cancellationToken);
        await EnsureLotsAsync(customer.Id, _options.LotsPerProduct, _options.LotExpirationWindowDays, cancellationToken);
        await EnsureInboundFlowAsync(customer.Id, warehouse, locations, cancellationToken);
        await EnsureInventoryBalancesAndMovementsAsync(customer.Id, locations, cancellationToken);
        await EnsureInventoryCountsAsync(customer.Id, warehouse.Id, cancellationToken);
        await EnsurePickingTasksAsync(customer.Id, warehouse, locations, crossDockLocations, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Sample data seed completed for customer {CustomerId} with {ProductCount} products.",
            customer.Id,
            _options.ProductCount);
    }

    private async Task CleanupSeedDataAsync(CancellationToken cancellationToken)
    {
        // Hard-delete seed inventory counts to avoid soft-delete (IsActive=false) leftovers.
        await _dbContext.Database.ExecuteSqlRawAsync(
            "DELETE FROM InventoryCountItems WHERE InventoryCountId IN (SELECT Id FROM InventoryCounts WHERE Notes LIKE {0})",
            new object[] { "SEED-COUNT-%" },
            cancellationToken);
        await _dbContext.Database.ExecuteSqlRawAsync(
            "DELETE FROM InventoryCounts WHERE Notes LIKE {0}",
            new object[] { "SEED-COUNT-%" },
            cancellationToken);

        var seedReceipts = await _dbContext.Receipts
            .Where(r => r.ReceiptNumber != null && EF.Functions.Like(r.ReceiptNumber, "RCV-SEED-INB-%"))
            .ToListAsync(cancellationToken);

        var seedReceiptIds = seedReceipts.Select(r => r.Id).ToList();
        if (seedReceiptIds.Count == 0)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var seedReceiptItems = await _dbContext.ReceiptItems
            .Where(ri => seedReceiptIds.Contains(ri.ReceiptId))
            .ToListAsync(cancellationToken);

        var balanceKeys = seedReceiptItems
            .Select(ri => new BalanceKey(ri.LocationId, ri.ProductId, ri.LotId))
            .Distinct()
            .ToList();

        if (balanceKeys.Count > 0)
        {
            var locationIds = balanceKeys.Select(k => k.LocationId).Distinct().ToList();
            var productIds = balanceKeys.Select(k => k.ProductId).Distinct().ToList();

            var balances = await _dbContext.InventoryBalances
                .Where(b => locationIds.Contains(b.LocationId) && productIds.Contains(b.ProductId))
                .ToListAsync(cancellationToken);

            var balanceLookup = new HashSet<BalanceKey>(balanceKeys);
            var balancesToRemove = balances
                .Where(b => balanceLookup.Contains(new BalanceKey(b.LocationId, b.ProductId, b.LotId)))
                .ToList();

            if (balancesToRemove.Count > 0)
            {
                _dbContext.InventoryBalances.RemoveRange(balancesToRemove);
            }
        }

        var seedPutawayTasks = await _dbContext.PutawayTasks
            .Where(t => seedReceiptIds.Contains(t.ReceiptId))
            .ToListAsync(cancellationToken);

        var seedUnitLoads = await _dbContext.UnitLoads
            .Where(ul => seedReceiptIds.Contains(ul.ReceiptId) && EF.Functions.Like(ul.SsccInternal, "SSCC-SEED-INB-%"))
            .ToListAsync(cancellationToken);

        var seedInboundOrders = await _dbContext.InboundOrders
            .Where(o => o.OrderNumber != null && EF.Functions.Like(o.OrderNumber, "OE-SEED-INB-%"))
            .ToListAsync(cancellationToken);

        var seedInboundOrderIds = seedInboundOrders.Select(o => o.Id).ToList();
        var seedInboundItems = await _dbContext.InboundOrderItems
            .Where(i => seedInboundOrderIds.Contains(i.InboundOrderId))
            .ToListAsync(cancellationToken);

        var seedAsns = await _dbContext.Asns
            .Where(a => a.AsnNumber != null && EF.Functions.Like(a.AsnNumber, "ASN-SEED-INB-%"))
            .ToListAsync(cancellationToken);

        var seedAsnIds = seedAsns.Select(a => a.Id).ToList();
        var seedAsnItems = await _dbContext.AsnItems
            .Where(i => seedAsnIds.Contains(i.AsnId))
            .ToListAsync(cancellationToken);

        _dbContext.PutawayTasks.RemoveRange(seedPutawayTasks);
        _dbContext.UnitLoads.RemoveRange(seedUnitLoads);
        _dbContext.ReceiptItems.RemoveRange(seedReceiptItems);
        _dbContext.Receipts.RemoveRange(seedReceipts);
        _dbContext.InboundOrderItems.RemoveRange(seedInboundItems);
        _dbContext.InboundOrders.RemoveRange(seedInboundOrders);
        _dbContext.AsnItems.RemoveRange(seedAsnItems);
        _dbContext.Asns.RemoveRange(seedAsns);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Customer> EnsureCustomerAsync(CancellationToken cancellationToken)
    {
        var customer = await _dbContext.Customers
            .FirstOrDefaultAsync(c => c.Id == _options.CustomerId, cancellationToken);

        if (customer is not null)
        {
            return customer;
        }

        customer = new Customer
        {
            Id = _options.CustomerId,
            Name = _options.CustomerName.Trim(),
            Email = _options.CustomerEmail.Trim().ToLowerInvariant(),
            DateOfBirth = new DateOnly(1990, 1, 1)
        };

        _dbContext.Customers.Add(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return customer;
    }

    private async Task<(Uom BaseUom, Uom BoxUom)> EnsureUomsAsync(CancellationToken cancellationToken)
    {
        var baseUom = await _dbContext.Uoms.FirstOrDefaultAsync(u => u.Code == "EA", cancellationToken);
        var boxUom = await _dbContext.Uoms.FirstOrDefaultAsync(u => u.Code == "BOX", cancellationToken);

        if (baseUom is null)
        {
            baseUom = new Uom
            {
                Id = Guid.NewGuid(),
                Code = "EA",
                Name = "Each",
                Type = UomType.Unit
            };
            _dbContext.Uoms.Add(baseUom);
        }

        if (boxUom is null)
        {
            boxUom = new Uom
            {
                Id = Guid.NewGuid(),
                Code = "BOX",
                Name = "Box",
                Type = UomType.Unit
            };
            _dbContext.Uoms.Add(boxUom);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return (baseUom, boxUom);
    }

    private Warehouse BuildWarehouse(string normalizedCode)
    {
        var warehouse = new Warehouse
        {
            Id = Guid.NewGuid(),
            Code = normalizedCode,
            Name = _options.WarehouseName.Trim(),
            ShortName = "DEMO",
            Description = "Sample warehouse seeded for demo visibility.",
            WarehouseType = WarehouseType.DistributionCenter,
            IsPrimary = true,
            IsPickingEnabled = true,
            IsReceivingEnabled = true,
            IsShippingEnabled = true,
            IsReturnsEnabled = true,
            ExternalId = "WH-DEMO-001",
            ErpCode = "ERP-DEMO",
            CostCenterCode = "CC-DEMO",
            CostCenterName = "Demo Operations",
            CutoffTime = new TimeOnly(18, 0),
            Timezone = "America/Sao_Paulo"
        };

        warehouse.Addresses.Add(new WarehouseAddress
        {
            Id = Guid.NewGuid(),
            AddressLine1 = "Av. Demo, 1000",
            AddressLine2 = "Bloco A",
            District = "Centro",
            City = "Sao Paulo",
            State = "SP",
            PostalCode = "01000-000",
            Country = "BR",
            Latitude = -23.55052m,
            Longitude = -46.633308m,
            IsPrimary = true
        });

        warehouse.Contacts.Add(new WarehouseContact
        {
            Id = Guid.NewGuid(),
            ContactName = "Operations",
            ContactEmail = "ops@demo.local",
            ContactPhone = "+55 11 99999-0000",
            IsPrimary = true
        });

        warehouse.Capacities.Add(new WarehouseCapacity
        {
            Id = Guid.NewGuid(),
            LengthMeters = 120,
            WidthMeters = 80,
            HeightMeters = 12,
            TotalAreaM2 = 9600,
            TotalCapacity = 1200,
            CapacityUnit = CapacityUnit.Pallets,
            MaxWeightKg = 150000,
            OperationalArea = 8000,
            IsPrimary = true
        });

        return warehouse;
    }

    private static Sector BuildSector(Guid warehouseId)
        => new()
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = "SEC-01",
            Name = "Storage Sector",
            Description = "Primary storage sector",
            SectorType = SectorType.Storage
        };

    private static Section BuildSection(Guid sectorId)
        => new()
        {
            Id = Guid.NewGuid(),
            SectorId = sectorId,
            Code = "SEC-A",
            Name = "Section A",
            Description = "Demo section"
        };

    private static Structure BuildStructure(Guid sectionId)
        => new()
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Code = "STR-01",
            Name = "Rack 01",
            StructureType = StructureType.SelectiveRack,
            Levels = 4
        };

    private static Aisle BuildAisle(Guid sectionId)
        => new()
        {
            Id = Guid.NewGuid(),
            SectionId = sectionId,
            Code = "AIS-01",
            Name = "Main Aisle"
        };

    private static List<Location> BuildLocations(Guid structureId, Guid zoneId)
    {
        var locations = new List<Location>();

        for (var level = 1; level <= 2; level++)
        {
            for (var row = 1; row <= 2; row++)
            {
                for (var column = 1; column <= 3; column++)
                {
                    var code = $"L{level:00}-R{row:00}-C{column:00}";
                    locations.Add(new Location
                    {
                        Id = Guid.NewGuid(),
                        StructureId = structureId,
                        ZoneId = zoneId,
                        Code = code,
                        Barcode = $"BC-{code}",
                        Level = level,
                        Row = row,
                        Column = column,
                        MaxWeightKg = 1000,
                        MaxVolumeM3 = 2.5m,
                        AllowLotTracking = true,
                        AllowExpiryTracking = true
                    });
                }
            }
        }

        return locations;
    }

    private static Zone BuildZone(Guid warehouseId)
        => new()
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = "ZON-01",
            Name = "General Storage",
            Description = "Default zone for demo locations.",
            ZoneType = ZoneType.Storage
        };

    private static Zone BuildQuarantineZone(Guid warehouseId)
        => new()
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = "ZON-QA",
            Name = "Quarantine",
            Description = "Quarantine zone for quality inspections.",
            ZoneType = ZoneType.Quarantine
        };

    private static Zone BuildCrossDockZone(Guid warehouseId)
        => new()
        {
            Id = Guid.NewGuid(),
            WarehouseId = warehouseId,
            Code = "ZON-CD",
            Name = "Cross-dock",
            Description = "Cross-dock zone for immediate dispatch.",
            ZoneType = ZoneType.CrossDock
        };

    private static List<Location> BuildQuarantineLocations(Guid structureId, Guid zoneId)
        => new()
        {
            new Location
            {
                Id = Guid.NewGuid(),
                StructureId = structureId,
                ZoneId = zoneId,
                Code = "Q-LOC-01",
                Barcode = "Q-LOC-01",
                Level = 0,
                Row = 0,
                Column = 1,
                MaxWeightKg = 500,
                MaxVolumeM3 = 1.5m,
                AllowLotTracking = true,
                AllowExpiryTracking = true
            }
        };

    private static List<Location> BuildCrossDockLocations(Guid structureId, Guid zoneId)
        => new()
        {
            new Location
            {
                Id = Guid.NewGuid(),
                StructureId = structureId,
                ZoneId = zoneId,
                Code = "CD-01",
                Barcode = "CD-01",
                Level = 1,
                Row = 1,
                Column = 1,
                MaxWeightKg = 500,
                MaxVolumeM3 = 1.5m,
                AllowLotTracking = true,
                AllowExpiryTracking = true
            },
            new Location
            {
                Id = Guid.NewGuid(),
                StructureId = structureId,
                ZoneId = zoneId,
                Code = "CD-02",
                Barcode = "CD-02",
                Level = 1,
                Row = 1,
                Column = 2,
                MaxWeightKg = 500,
                MaxVolumeM3 = 1.5m,
                AllowLotTracking = true,
                AllowExpiryTracking = true
            }
        };

    private static (List<Product> Products, List<ProductUom> ProductUoms) BuildProducts(
        Guid customerId,
        Guid baseUomId,
        Guid boxUomId,
        int count)
    {
        var products = new List<Product>();
        var productUoms = new List<ProductUom>();

        for (var i = 1; i <= count; i++)
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Code = $"SKU-{i:000}",
                Name = $"Sample Product {i:000}",
                Description = $"Sample product {i:000} for demo visibility.",
                Ean = $"789000000{i:000}",
                ErpCode = $"ERP-{i:000}",
                Category = "Demo",
                Brand = "Devcraft",
                BaseUomId = baseUomId,
                TrackingMode = DevcraftWMS.Domain.Enums.TrackingMode.LotAndExpiry,
                MinimumShelfLifeDays = 30,
                WeightKg = 1.2m + i * 0.1m,
                LengthCm = 20 + i,
                WidthCm = 15 + i,
                HeightCm = 10 + i,
                VolumeCm3 = (20 + i) * (15 + i) * (10 + i)
            };

            products.Add(product);

            productUoms.Add(new ProductUom
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = product.Id,
                UomId = baseUomId,
                ConversionFactor = 1,
                IsBase = true
            });

            productUoms.Add(new ProductUom
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ProductId = product.Id,
                UomId = boxUomId,
                ConversionFactor = 10,
                IsBase = false
            });
        }

        return (products, productUoms);
    }

    private async Task<Warehouse> EnsureWarehouseAsync(string normalizedCode, CancellationToken cancellationToken)
    {
        var warehouse = await _dbContext.Warehouses
            .Include(w => w.Addresses)
            .Include(w => w.Contacts)
            .Include(w => w.Capacities)
            .FirstOrDefaultAsync(w => w.Code == normalizedCode, cancellationToken);

        if (warehouse is not null)
        {
            return warehouse;
        }

        warehouse = BuildWarehouse(normalizedCode);
        _dbContext.Warehouses.Add(warehouse);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return warehouse;
    }

    private async Task<Sector> EnsureSectorAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var sector = await _dbContext.Sectors
            .Include(s => s.CustomerAccesses)
            .FirstOrDefaultAsync(s => s.WarehouseId == warehouseId && s.Code == "SEC-01", cancellationToken);

        if (sector is not null)
        {
            return sector;
        }

        sector = BuildSector(warehouseId);
        _dbContext.Sectors.Add(sector);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return sector;
    }

    private async Task<Section> EnsureSectionAsync(Guid sectorId, CancellationToken cancellationToken)
    {
        var section = await _dbContext.Sections
            .Include(s => s.CustomerAccesses)
            .FirstOrDefaultAsync(s => s.SectorId == sectorId && s.Code == "SEC-A", cancellationToken);

        if (section is not null)
        {
            return section;
        }

        section = BuildSection(sectorId);
        _dbContext.Sections.Add(section);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return section;
    }

    private async Task<Structure> EnsureStructureAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        var structure = await _dbContext.Structures
            .Include(s => s.CustomerAccesses)
            .FirstOrDefaultAsync(s => s.SectionId == sectionId && s.Code == "STR-01", cancellationToken);

        if (structure is not null)
        {
            return structure;
        }

        structure = BuildStructure(sectionId);
        _dbContext.Structures.Add(structure);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return structure;
    }

    private async Task<Aisle> EnsureAisleAsync(Guid sectionId, CancellationToken cancellationToken)
    {
        var aisle = await _dbContext.Aisles
            .Include(a => a.CustomerAccesses)
            .FirstOrDefaultAsync(a => a.SectionId == sectionId && a.Code == "AIS-01", cancellationToken);

        if (aisle is not null)
        {
            return aisle;
        }

        aisle = BuildAisle(sectionId);
        _dbContext.Aisles.Add(aisle);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return aisle;
    }

    private async Task<List<Location>> EnsureLocationsAsync(Guid structureId, Guid zoneId, CancellationToken cancellationToken)
    {
        var expected = BuildLocations(structureId, zoneId);
        var existing = await _dbContext.Locations
            .Include(l => l.CustomerAccesses)
            .Where(l => l.StructureId == structureId)
            .ToListAsync(cancellationToken);

        var existingCodes = existing.Select(l => l.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = expected.Where(l => !existingCodes.Contains(l.Code)).ToList();

        if (missing.Count > 0)
        {
            _dbContext.Locations.AddRange(missing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            existing.AddRange(missing);
        }

        return existing;
    }

    private async Task<Zone> EnsureZoneAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var zone = await _dbContext.Zones.FirstOrDefaultAsync(z => z.WarehouseId == warehouseId, cancellationToken);
        if (zone is not null)
        {
            return zone;
        }

        zone = BuildZone(warehouseId);
        _dbContext.Zones.Add(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return zone;
    }

    private async Task<Zone> EnsureQuarantineZoneAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var zone = await _dbContext.Zones
            .FirstOrDefaultAsync(z => z.WarehouseId == warehouseId && z.ZoneType == ZoneType.Quarantine, cancellationToken);

        if (zone is not null)
        {
            return zone;
        }

        zone = BuildQuarantineZone(warehouseId);
        _dbContext.Zones.Add(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return zone;
    }

    private async Task<Zone> EnsureCrossDockZoneAsync(Guid warehouseId, CancellationToken cancellationToken)
    {
        var zone = await _dbContext.Zones
            .FirstOrDefaultAsync(z => z.WarehouseId == warehouseId && z.ZoneType == ZoneType.CrossDock, cancellationToken);

        if (zone is not null)
        {
            return zone;
        }

        zone = BuildCrossDockZone(warehouseId);
        _dbContext.Zones.Add(zone);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return zone;
    }

    private async Task<List<Location>> EnsureQuarantineLocationsAsync(Guid structureId, Guid zoneId, CancellationToken cancellationToken)
    {
        var expected = BuildQuarantineLocations(structureId, zoneId);
        var existing = await _dbContext.Locations
            .Include(l => l.CustomerAccesses)
            .Where(l => l.StructureId == structureId && l.ZoneId == zoneId)
            .ToListAsync(cancellationToken);

        var existingCodes = existing.Select(l => l.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = expected.Where(l => !existingCodes.Contains(l.Code)).ToList();

        if (missing.Count > 0)
        {
            _dbContext.Locations.AddRange(missing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            existing.AddRange(missing);
        }

        return existing;
    }

    private async Task<List<Location>> EnsureCrossDockLocationsAsync(Guid structureId, Guid zoneId, CancellationToken cancellationToken)
    {
        var expected = BuildCrossDockLocations(structureId, zoneId);
        var existing = await _dbContext.Locations
            .Include(l => l.CustomerAccesses)
            .Where(l => l.StructureId == structureId && l.ZoneId == zoneId)
            .ToListAsync(cancellationToken);

        var existingCodes = existing.Select(l => l.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var missing = expected.Where(l => !existingCodes.Contains(l.Code)).ToList();

        if (missing.Count > 0)
        {
            _dbContext.Locations.AddRange(missing);
            await _dbContext.SaveChangesAsync(cancellationToken);
            existing.AddRange(missing);
        }

        return existing;
    }

    private async Task EnsureSectorAccessAsync(Sector sector, Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.SectorCustomers
            .AnyAsync(s => s.SectorId == sector.Id && s.CustomerId == customerId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.SectorCustomers.Add(new SectorCustomer
        {
            Id = Guid.NewGuid(),
            SectorId = sector.Id,
            CustomerId = customerId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSectionAccessAsync(Section section, Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.SectionCustomers
            .AnyAsync(s => s.SectionId == section.Id && s.CustomerId == customerId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.SectionCustomers.Add(new SectionCustomer
        {
            Id = Guid.NewGuid(),
            SectionId = section.Id,
            CustomerId = customerId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStructureAccessAsync(Structure structure, Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.StructureCustomers
            .AnyAsync(s => s.StructureId == structure.Id && s.CustomerId == customerId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.StructureCustomers.Add(new StructureCustomer
        {
            Id = Guid.NewGuid(),
            StructureId = structure.Id,
            CustomerId = customerId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAisleAccessAsync(Aisle aisle, Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.AisleCustomers
            .AnyAsync(a => a.AisleId == aisle.Id && a.CustomerId == customerId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.AisleCustomers.Add(new AisleCustomer
        {
            Id = Guid.NewGuid(),
            AisleId = aisle.Id,
            CustomerId = customerId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureLocationAccessAsync(IEnumerable<Location> locations, Guid customerId, CancellationToken cancellationToken)
    {
        var locationIds = locations.Select(l => l.Id).ToList();
        if (locationIds.Count == 0)
        {
            return;
        }

        var existing = await _dbContext.LocationCustomers
            .Where(lc => lc.CustomerId == customerId && locationIds.Contains(lc.LocationId))
            .Select(lc => lc.LocationId)
            .ToListAsync(cancellationToken);

        var existingSet = existing.ToHashSet();
        var missingIds = locationIds.Where(id => !existingSet.Contains(id)).ToList();

        if (missingIds.Count == 0)
        {
            return;
        }

        foreach (var locationId in missingIds)
        {
            _dbContext.LocationCustomers.Add(new LocationCustomer
            {
                Id = Guid.NewGuid(),
                LocationId = locationId,
                CustomerId = customerId
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureZoneAccessAsync(Zone zone, Guid customerId, CancellationToken cancellationToken)
    {
        var exists = await _dbContext.ZoneCustomers
            .AnyAsync(z => z.ZoneId == zone.Id && z.CustomerId == customerId, cancellationToken);

        if (exists)
        {
            return;
        }

        _dbContext.ZoneCustomers.Add(new ZoneCustomer
        {
            Id = Guid.NewGuid(),
            ZoneId = zone.Id,
            CustomerId = customerId
        });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureProductsAsync(
        Guid customerId,
        Guid baseUomId,
        Guid boxUomId,
        int count,
        CancellationToken cancellationToken)
    {
        for (var i = 1; i <= count; i++)
        {
            var code = $"SKU-{i:000}";
            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.CustomerId == customerId && p.Code == code, cancellationToken);

            if (product is null)
            {
                product = new Product
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Code = code,
                    Name = $"Sample Product {i:000}",
                    Description = $"Sample product {i:000} for demo visibility.",
                    Ean = $"789000000{i:000}",
                    ErpCode = $"ERP-{i:000}",
                    Category = "Demo",
                    Brand = "Devcraft",
                    BaseUomId = baseUomId,
                    TrackingMode = DevcraftWMS.Domain.Enums.TrackingMode.LotAndExpiry,
                    MinimumShelfLifeDays = 30,
                    WeightKg = 1.2m + i * 0.1m,
                    LengthCm = 20 + i,
                    WidthCm = 15 + i,
                    HeightCm = 10 + i,
                    VolumeCm3 = (20 + i) * (15 + i) * (10 + i)
                };
                _dbContext.Products.Add(product);
            }
            else
            {
                if (product.BaseUomId == Guid.Empty)
                {
                    product.BaseUomId = baseUomId;
                }
            }

            var hasBaseUom = await _dbContext.ProductUoms.AnyAsync(
                pu => pu.CustomerId == customerId && pu.ProductId == product.Id && pu.UomId == baseUomId,
                cancellationToken);

            if (!hasBaseUom)
            {
                _dbContext.ProductUoms.Add(new ProductUom
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ProductId = product.Id,
                    UomId = baseUomId,
                    ConversionFactor = 1,
                    IsBase = true
                });
            }

            var hasBoxUom = await _dbContext.ProductUoms.AnyAsync(
                pu => pu.CustomerId == customerId && pu.ProductId == product.Id && pu.UomId == boxUomId,
                cancellationToken);

            if (!hasBoxUom)
            {
                _dbContext.ProductUoms.Add(new ProductUom
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ProductId = product.Id,
                    UomId = boxUomId,
                    ConversionFactor = 10,
                    IsBase = false
                });
            }
        }
    }

    private async Task EnsureLotsAsync(
        Guid customerId,
        int lotsPerProduct,
        int expirationWindowDays,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var products = await _dbContext.Products
            .Where(p => p.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        foreach (var product in products)
        {
            for (var i = 1; i <= lotsPerProduct; i++)
            {
                var code = $"LOT-{product.Code}-{i:00}";
                var exists = await _dbContext.Lots.AnyAsync(
                    l => l.ProductId == product.Id && l.Code == code,
                    cancellationToken);

                if (exists)
                {
                    continue;
                }

                var expirationDays = Math.Min(expirationWindowDays, 30 + (i * 5));
                var expirationDate = today.AddDays(expirationDays);
                var manufactureDate = today.AddDays(-15);

                _dbContext.Lots.Add(new Lot
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    Code = code,
                    ManufactureDate = manufactureDate,
                    ExpirationDate = expirationDate,
                    Status = LotStatus.Available
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureInboundFlowAsync(
        Guid customerId,
        Warehouse warehouse,
        IReadOnlyList<Location> locations,
        CancellationToken cancellationToken)
    {
        if (_options.InboundOrderCount <= 0)
        {
            return;
        }

        var existingSeedReceipts = await _dbContext.Receipts
            .AnyAsync(
                r => r.CustomerId == customerId &&
                     r.ReceiptNumber != null &&
                     EF.Functions.Like(r.ReceiptNumber, "RCV-SEED-INB-%"),
                cancellationToken);

        if (existingSeedReceipts)
        {
            return;
        }

        var products = await _dbContext.Products
            .Where(p => p.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        if (products.Count == 0 || locations.Count == 0)
        {
            return;
        }

        var productIds = products.Select(p => p.Id).ToList();
        var lots = await _dbContext.Lots
            .Where(l => productIds.Contains(l.ProductId))
            .ToListAsync(cancellationToken);
        var locationIds = locations.Select(l => l.Id).ToList();
        var lotIds = lots.Select(l => l.Id).ToList();
        var existingBalances = await _dbContext.InventoryBalances
            .Where(b => locationIds.Contains(b.LocationId) && productIds.Contains(b.ProductId))
            .ToListAsync(cancellationToken);
        var balanceLookup = existingBalances
            .ToDictionary(b => new BalanceKey(b.LocationId, b.ProductId, b.LotId));

        var random = new Random(91);
        var now = DateTime.UtcNow;

        var asns = new List<Asn>();
        var asnItems = new List<AsnItem>();
        var inboundOrders = new List<InboundOrder>();
        var inboundItems = new List<InboundOrderItem>();
        var receipts = new List<Receipt>();
        var receiptItems = new List<ReceiptItem>();
        var unitLoads = new List<UnitLoad>();
        var putawayTasks = new List<PutawayTask>();

        for (var i = 1; i <= _options.InboundOrderCount; i++)
        {
            var expectedArrival = DateOnly.FromDateTime(now.AddDays(-random.Next(1, 10)));
            var asn = new Asn
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                WarehouseId = warehouse.Id,
                AsnNumber = $"ASN-SEED-INB-{i:000}",
                SupplierName = "Seed Supplier",
                DocumentNumber = $"NF-SEED-{i:000}",
                ExpectedArrivalDate = expectedArrival,
                Notes = "Seed inbound flow",
                Status = AsnStatus.Converted
            };

            var inboundOrder = new InboundOrder
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                WarehouseId = warehouse.Id,
                AsnId = asn.Id,
                OrderNumber = $"OE-SEED-INB-{i:000}",
                SupplierName = asn.SupplierName,
                DocumentNumber = asn.DocumentNumber,
                ExpectedArrivalDate = asn.ExpectedArrivalDate,
                Status = InboundOrderStatus.Completed,
                Priority = InboundOrderPriority.Normal,
                InspectionLevel = InboundOrderInspectionLevel.None
            };

            var receiptStarted = now.AddHours(-random.Next(12, 72));
            var receipt = new Receipt
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                WarehouseId = warehouse.Id,
                InboundOrderId = inboundOrder.Id,
                ReceiptNumber = $"RCV-SEED-INB-{i:000}",
                DocumentNumber = asn.DocumentNumber,
                SupplierName = asn.SupplierName,
                Status = ReceiptStatus.Completed,
                StartedAtUtc = receiptStarted,
                ReceivedAtUtc = receiptStarted.AddHours(random.Next(2, 6)),
                Notes = "Seed receipt completed"
            };

            var selectedProducts = products
                .OrderBy(_ => random.Next())
                .Take(Math.Min(_options.ReceiptItemsPerOrder, products.Count))
                .ToList();

            foreach (var product in selectedProducts)
            {
                var lot = lots.Where(l => l.ProductId == product.Id).OrderBy(_ => random.Next()).FirstOrDefault();
                var quantity = Math.Max(5, random.Next(10, 120));
                var location = locations[random.Next(locations.Count)];

                asnItems.Add(new AsnItem
                {
                    Id = Guid.NewGuid(),
                    AsnId = asn.Id,
                    ProductId = product.Id,
                    UomId = product.BaseUomId,
                    Quantity = quantity,
                    LotCode = lot?.Code,
                    ExpirationDate = lot?.ExpirationDate
                });

                inboundItems.Add(new InboundOrderItem
                {
                    Id = Guid.NewGuid(),
                    InboundOrderId = inboundOrder.Id,
                    ProductId = product.Id,
                    UomId = product.BaseUomId,
                    Quantity = quantity,
                    LotCode = lot?.Code,
                    ExpirationDate = lot?.ExpirationDate
                });

                receiptItems.Add(new ReceiptItem
                {
                    Id = Guid.NewGuid(),
                    ReceiptId = receipt.Id,
                    ProductId = product.Id,
                    LotId = lot?.Id,
                    LocationId = location.Id,
                    UomId = product.BaseUomId,
                    Quantity = quantity,
                    UnitCost = 10 + random.Next(1, 50)
                });

                var lotId = lot?.Id;
                var balanceKey = new BalanceKey(location.Id, product.Id, lotId);
                if (!balanceLookup.TryGetValue(balanceKey, out var balance))
                {
                    balance = new InventoryBalance
                    {
                        Id = Guid.NewGuid(),
                        LocationId = location.Id,
                        ProductId = product.Id,
                        LotId = lotId,
                        QuantityOnHand = 0,
                        QuantityReserved = 0,
                        Status = InventoryBalanceStatus.Available
                    };
                    _dbContext.InventoryBalances.Add(balance);
                    balanceLookup.Add(balanceKey, balance);
                }

                balance.QuantityOnHand += quantity;
            }

            for (var u = 1; u <= _options.UnitLoadsPerOrder; u++)
            {
                var unitLoad = new UnitLoad
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    WarehouseId = warehouse.Id,
                    ReceiptId = receipt.Id,
                    SsccInternal = $"SSCC-SEED-INB-{i:000}-{u:00}",
                    Status = UnitLoadStatus.PutawayCompleted,
                    PrintedAtUtc = receipt.ReceivedAtUtc,
                    Notes = "Seed unit load"
                };

                unitLoads.Add(unitLoad);
                putawayTasks.Add(new PutawayTask
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    WarehouseId = warehouse.Id,
                    ReceiptId = receipt.Id,
                    UnitLoadId = unitLoad.Id,
                    Status = PutawayTaskStatus.Completed
                });
            }

            asns.Add(asn);
            inboundOrders.Add(inboundOrder);
            receipts.Add(receipt);
        }

        _dbContext.Asns.AddRange(asns);
        _dbContext.AsnItems.AddRange(asnItems);
        _dbContext.InboundOrders.AddRange(inboundOrders);
        _dbContext.InboundOrderItems.AddRange(inboundItems);
        _dbContext.Receipts.AddRange(receipts);
        _dbContext.ReceiptItems.AddRange(receiptItems);
        _dbContext.UnitLoads.AddRange(unitLoads);
        _dbContext.PutawayTasks.AddRange(putawayTasks);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private readonly record struct BalanceKey(Guid LocationId, Guid ProductId, Guid? LotId);

    private async Task EnsureInventoryCountsAsync(Guid customerId, Guid warehouseId, CancellationToken cancellationToken)
    {
        if (_options.InventoryCountCount <= 0)
        {
            return;
        }

        var existingSeedCounts = await _dbContext.InventoryCounts
            .AnyAsync(c => c.Notes != null && EF.Functions.Like(c.Notes, "SEED-COUNT-%"), cancellationToken);

        if (existingSeedCounts)
        {
            return;
        }

        var balances = await _dbContext.InventoryBalances
            .Include(b => b.Location)
            .Include(b => b.Product)
            .Where(b => b.Location != null && b.Product != null)
            .ToListAsync(cancellationToken);

        if (balances.Count == 0)
        {
            return;
        }

        var random = new Random(123);
        var counts = new List<InventoryCount>();
        var countItems = new List<InventoryCountItem>();

        for (var i = 1; i <= _options.InventoryCountCount; i++)
        {
            var balanceSample = balances[random.Next(balances.Count)];
            var count = new InventoryCount
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                WarehouseId = warehouseId,
                LocationId = balanceSample.LocationId,
                ZoneId = balanceSample.Location?.ZoneId,
                Status = InventoryCountStatus.Draft,
                Notes = $"SEED-COUNT-{i:000}",
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            counts.Add(count);

            var selectedBalances = balances
                .OrderBy(_ => random.Next())
                .Take(Math.Min(_options.InventoryCountItemsPerCount, balances.Count))
                .ToList();

            foreach (var balance in selectedBalances)
            {
                if (balance.Product is null)
                {
                    continue;
                }

                countItems.Add(new InventoryCountItem
                {
                    Id = Guid.NewGuid(),
                    InventoryCountId = count.Id,
                    LocationId = balance.LocationId,
                    ProductId = balance.ProductId,
                    UomId = balance.Product.BaseUomId,
                    LotId = balance.LotId,
                    QuantityExpected = balance.QuantityOnHand,
                    QuantityCounted = 0,
                    IsActive = true,
                });
            }
        }

        _dbContext.InventoryCounts.AddRange(counts);
        _dbContext.InventoryCountItems.AddRange(countItems);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureInventoryBalancesAndMovementsAsync(Guid customerId, IReadOnlyList<Location> locations, CancellationToken cancellationToken)
    {
        if (_options.MovementCount <= 0)
        {
            return;
        }

        var existingSeedMovements = await _dbContext.InventoryMovements
            .AnyAsync(
                m => m.CustomerId == customerId &&
                     m.Reference != null &&
                     EF.Functions.Like(m.Reference, "SEED-MOVE-%"),
                cancellationToken);

        if (existingSeedMovements)
        {
            return;
        }

        var products = await _dbContext.Products
            .Where(p => p.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        if (products.Count == 0 || locations.Count < 2)
        {
            return;
        }

        var lots = await _dbContext.Lots
            .Where(l => products.Select(p => p.Id).Contains(l.ProductId))
            .ToListAsync(cancellationToken);

        var random = new Random(42);
        var now = DateTime.UtcNow;

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        foreach (var product in products)
        {
            var location = locations[random.Next(locations.Count)];
            var lot = lots.FirstOrDefault(l => l.ProductId == product.Id);
            var lotId = lot?.Id;
            var existingBalance = await _dbContext.InventoryBalances
                .FirstOrDefaultAsync(b => b.LocationId == location.Id && b.ProductId == product.Id && b.LotId == lotId, cancellationToken);

            if (existingBalance is null)
            {
                _dbContext.InventoryBalances.Add(new InventoryBalance
                {
                    Id = Guid.NewGuid(),
                    LocationId = location.Id,
                    ProductId = product.Id,
                    LotId = lotId,
                    QuantityOnHand = Math.Max(_options.MovementQuantityMax * 2, 50),
                    QuantityReserved = 0,
                    Status = InventoryBalanceStatus.Available
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        var balances = await _dbContext.InventoryBalances
            .Where(b => products.Select(p => p.Id).Contains(b.ProductId))
            .ToListAsync(cancellationToken);

        var movementsToCreate = new List<InventoryMovement>();

        for (var i = 1; i <= _options.MovementCount; i++)
        {
            var balance = balances
                .OrderByDescending(b => b.QuantityOnHand)
                .FirstOrDefault();

            if (balance is null || balance.QuantityOnHand <= _options.MovementQuantityMin)
            {
                break;
            }

            var toLocation = locations.FirstOrDefault(l => l.Id != balance.LocationId);
            if (toLocation is null)
            {
                break;
            }

            var quantity = Math.Min(balance.QuantityOnHand, RandomDecimal(random, _options.MovementQuantityMin, _options.MovementQuantityMax));
            if (quantity <= 0)
            {
                continue;
            }

            balance.QuantityOnHand -= quantity;

            var destinationBalance = balances.FirstOrDefault(b =>
                b.LocationId == toLocation.Id &&
                b.ProductId == balance.ProductId &&
                b.LotId == balance.LotId);

            if (destinationBalance is null)
            {
                destinationBalance = new InventoryBalance
                {
                    Id = Guid.NewGuid(),
                    LocationId = toLocation.Id,
                    ProductId = balance.ProductId,
                    LotId = balance.LotId,
                    QuantityOnHand = 0,
                    QuantityReserved = 0,
                    Status = balance.Status
                };
                _dbContext.InventoryBalances.Add(destinationBalance);
                balances.Add(destinationBalance);
            }

            destinationBalance.QuantityOnHand += quantity;

            movementsToCreate.Add(new InventoryMovement
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                FromLocationId = balance.LocationId,
                ToLocationId = toLocation.Id,
                ProductId = balance.ProductId,
                LotId = balance.LotId,
                Quantity = quantity,
                Reason = "Seed movement",
                Reference = $"SEED-MOVE-{i:000}",
                Status = InventoryMovementStatus.Completed,
                PerformedAtUtc = now.AddDays(-random.Next(0, _options.MovementPerformedWindowDays))
            });
        }

        if (movementsToCreate.Count > 0)
        {
            _dbContext.InventoryMovements.AddRange(movementsToCreate);
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private static decimal RandomDecimal(Random random, decimal min, decimal max)
    {
        if (max <= min)
        {
            return min;
        }

        var next = (decimal)random.NextDouble();
        return Math.Round(min + (max - min) * next, 3);
    }

    private async Task EnsurePickingTasksAsync(
        Guid customerId,
        Warehouse warehouse,
        IReadOnlyList<Location> locations,
        IReadOnlyList<Location> crossDockLocations,
        CancellationToken cancellationToken)
    {
        if (_options.PickingTaskCount <= 0)
        {
            return;
        }

        var existingSeedTasks = await _dbContext.PickingTasks
            .Include(t => t.OutboundOrder)
            .AnyAsync(t => t.OutboundOrder != null && EF.Functions.Like(t.OutboundOrder.OrderNumber, "OS-SEED-PICK-%"), cancellationToken);

        if (existingSeedTasks)
        {
            return;
        }

        var products = await _dbContext.Products
            .Where(p => p.CustomerId == customerId)
            .ToListAsync(cancellationToken);

        if (products.Count == 0 || locations.Count == 0)
        {
            return;
        }

        var productIds = products.Select(p => p.Id).ToList();
        var lots = await _dbContext.Lots
            .Where(l => productIds.Contains(l.ProductId))
            .ToListAsync(cancellationToken);

        var random = new Random(77);
        var now = DateTime.UtcNow;

        var orderCount = Math.Clamp(_options.PickingTaskCount / 3, 2, 4);
        var orders = new List<OutboundOrder>();
        var orderItems = new List<OutboundOrderItem>();

        for (var i = 1; i <= orderCount; i++)
        {
            var isCrossDock = i % 3 == 0;
            var order = new OutboundOrder
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                WarehouseId = warehouse.Id,
                OrderNumber = $"OS-SEED-PICK-{i:000}",
                Status = OutboundOrderStatus.Picking,
                Priority = i % 2 == 0 ? OutboundOrderPriority.High : OutboundOrderPriority.Normal,
                PickingMethod = (OutboundOrderPickingMethod)(i % 4),
                IsCrossDock = isCrossDock
            };

            orders.Add(order);

            var itemsCount = 2 + (i % 2);
            for (var j = 0; j < itemsCount; j++)
            {
                var product = products[(i + j) % products.Count];
                var lot = lots.FirstOrDefault(l => l.ProductId == product.Id);
                var quantity = 2 + random.Next(1, 6);

                orderItems.Add(new OutboundOrderItem
                {
                    Id = Guid.NewGuid(),
                    OutboundOrderId = order.Id,
                    ProductId = product.Id,
                    UomId = product.BaseUomId,
                    Quantity = quantity,
                    LotCode = lot?.Code,
                    ExpirationDate = lot?.ExpirationDate
                });
            }
        }

        _dbContext.OutboundOrders.AddRange(orders);
        _dbContext.OutboundOrderItems.AddRange(orderItems);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var statusCycle = new[]
        {
            PickingTaskStatus.Pending,
            PickingTaskStatus.InProgress,
            PickingTaskStatus.Completed,
            PickingTaskStatus.Pending,
            PickingTaskStatus.Reassigned,
            PickingTaskStatus.Canceled
        };

        var tasks = new List<PickingTask>();
        var taskItems = new List<PickingTaskItem>();

        for (var i = 0; i < _options.PickingTaskCount; i++)
        {
            var order = orders[i % orders.Count];
            var itemsForOrder = orderItems.Where(oi => oi.OutboundOrderId == order.Id).ToList();
            if (itemsForOrder.Count == 0)
            {
                continue;
            }

            var status = statusCycle[i % statusCycle.Length];
            var task = new PickingTask
            {
                Id = Guid.NewGuid(),
                OutboundOrderId = order.Id,
                WarehouseId = warehouse.Id,
                Sequence = i + 1,
                Status = status,
                StartedAtUtc = status is PickingTaskStatus.InProgress or PickingTaskStatus.Completed
                    ? now.AddMinutes(-30 * (i + 1))
                    : null,
                CompletedAtUtc = status == PickingTaskStatus.Completed
                    ? now.AddMinutes(-15 * (i + 1))
                    : null,
                Notes = status == PickingTaskStatus.Reassigned ? "Reassigned during seed." : null
            };

            tasks.Add(task);

            var orderItem = itemsForOrder[i % itemsForOrder.Count];
            var locationPool = order.IsCrossDock && crossDockLocations.Count > 0 ? crossDockLocations : locations;
            var location = locationPool[random.Next(locationPool.Count)];
            var picked = status == PickingTaskStatus.Completed
                ? orderItem.Quantity
                : status == PickingTaskStatus.InProgress
                    ? Math.Max(0, orderItem.Quantity - 1)
                    : 0;

            taskItems.Add(new PickingTaskItem
            {
                Id = Guid.NewGuid(),
                PickingTaskId = task.Id,
                OutboundOrderItemId = orderItem.Id,
                ProductId = orderItem.ProductId,
                UomId = orderItem.UomId,
                LotId = lots.FirstOrDefault(l => l.Code == orderItem.LotCode && l.ProductId == orderItem.ProductId)?.Id,
                LocationId = location.Id,
                QuantityPlanned = orderItem.Quantity,
                QuantityPicked = picked
            });
        }

        if (orders.Count > 0)
        {
            var completedOrderId = orders[0].Id;
            foreach (var task in tasks.Where(t => t.OutboundOrderId == completedOrderId))
            {
                task.Status = PickingTaskStatus.Completed;
                task.StartedAtUtc ??= now.AddMinutes(-60);
                task.CompletedAtUtc = now.AddMinutes(-30);
            }

            foreach (var item in taskItems.Where(i => tasks.Any(t => t.Id == i.PickingTaskId && t.OutboundOrderId == completedOrderId)))
            {
                item.QuantityPicked = item.QuantityPlanned;
            }
        }

        _dbContext.PickingTasks.AddRange(tasks);
        _dbContext.PickingTaskItems.AddRange(taskItems);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await EnsureOutboundChecksAsync(customerId, orders, tasks, cancellationToken);
    }

    private async Task EnsureOutboundChecksAsync(
        Guid customerId,
        IReadOnlyList<OutboundOrder> orders,
        IReadOnlyList<PickingTask> tasks,
        CancellationToken cancellationToken)
    {
        if (orders.Count == 0 || tasks.Count == 0)
        {
            return;
        }

        var completedOrders = tasks
            .GroupBy(t => t.OutboundOrderId)
            .Where(g => g.All(t => t.Status == PickingTaskStatus.Completed))
            .Select(g => g.Key)
            .ToHashSet();

        if (completedOrders.Count == 0)
        {
            return;
        }

        var existingChecks = await _dbContext.OutboundChecks
            .Where(c => c.CustomerId == customerId && completedOrders.Contains(c.OutboundOrderId))
            .Select(c => c.OutboundOrderId)
            .ToListAsync(cancellationToken);

        var existingSet = existingChecks.ToHashSet();
        var candidates = orders
            .Where(o => completedOrders.Contains(o.Id) && !existingSet.Contains(o.Id))
            .ToList();

        if (candidates.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        var checks = new List<OutboundCheck>();

        for (var i = 0; i < candidates.Count; i++)
        {
            var order = candidates[i];
            var status = i == 0 ? OutboundCheckStatus.InProgress : OutboundCheckStatus.Pending;
            checks.Add(new OutboundCheck
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                OutboundOrderId = order.Id,
                WarehouseId = order.WarehouseId,
                Status = status,
                Priority = order.Priority,
                StartedAtUtc = status == OutboundCheckStatus.InProgress ? now.AddMinutes(-10) : null
            });
        }

        _dbContext.OutboundChecks.AddRange(checks);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
