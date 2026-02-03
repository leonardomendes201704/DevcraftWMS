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

        var normalizedWarehouseCode = _options.WarehouseCode.Trim().ToUpperInvariant();
        var customer = await EnsureCustomerAsync(cancellationToken);
        var uoms = await EnsureUomsAsync(cancellationToken);

        var warehouse = await EnsureWarehouseAsync(normalizedWarehouseCode, cancellationToken);
        var sector = await EnsureSectorAsync(warehouse.Id, cancellationToken);
        var section = await EnsureSectionAsync(sector.Id, cancellationToken);
        var structure = await EnsureStructureAsync(section.Id, cancellationToken);
        var aisle = await EnsureAisleAsync(section.Id, cancellationToken);
        var zone = await EnsureZoneAsync(warehouse.Id, cancellationToken);
        var locations = await EnsureLocationsAsync(structure.Id, zone.Id, cancellationToken);

        await EnsureSectorAccessAsync(sector, customer.Id, cancellationToken);
        await EnsureSectionAccessAsync(section, customer.Id, cancellationToken);
        await EnsureStructureAccessAsync(structure, customer.Id, cancellationToken);
        await EnsureAisleAccessAsync(aisle, customer.Id, cancellationToken);
        await EnsureZoneAccessAsync(zone, customer.Id, cancellationToken);
        await EnsureLocationAccessAsync(locations, customer.Id, cancellationToken);

        await EnsureProductsAsync(customer.Id, uoms.BaseUom.Id, uoms.BoxUom.Id, _options.ProductCount, cancellationToken);
        await EnsureLotsAsync(customer.Id, _options.LotsPerProduct, _options.LotExpirationWindowDays, cancellationToken);
        await EnsureInventoryBalancesAndMovementsAsync(customer.Id, locations, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation(
            "Sample data seed completed for customer {CustomerId} with {ProductCount} products.",
            customer.Id,
            _options.ProductCount);
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

    private async Task EnsureSectorAccessAsync(Sector sector, Guid customerId, CancellationToken cancellationToken)
    {
        if (sector.CustomerAccesses.Any(a => a.CustomerId == customerId))
        {
            return;
        }

        sector.CustomerAccesses.Add(new SectorCustomer { Id = Guid.NewGuid(), CustomerId = customerId });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureSectionAccessAsync(Section section, Guid customerId, CancellationToken cancellationToken)
    {
        if (section.CustomerAccesses.Any(a => a.CustomerId == customerId))
        {
            return;
        }

        section.CustomerAccesses.Add(new SectionCustomer { Id = Guid.NewGuid(), CustomerId = customerId });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStructureAccessAsync(Structure structure, Guid customerId, CancellationToken cancellationToken)
    {
        if (structure.CustomerAccesses.Any(a => a.CustomerId == customerId))
        {
            return;
        }

        structure.CustomerAccesses.Add(new StructureCustomer { Id = Guid.NewGuid(), CustomerId = customerId });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureAisleAccessAsync(Aisle aisle, Guid customerId, CancellationToken cancellationToken)
    {
        if (aisle.CustomerAccesses.Any(a => a.CustomerId == customerId))
        {
            return;
        }

        aisle.CustomerAccesses.Add(new AisleCustomer { Id = Guid.NewGuid(), CustomerId = customerId });
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureLocationAccessAsync(IEnumerable<Location> locations, Guid customerId, CancellationToken cancellationToken)
    {
        var missing = locations
            .Where(l => l.CustomerAccesses.All(a => a.CustomerId != customerId))
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        foreach (var location in missing)
        {
            location.CustomerAccesses.Add(new LocationCustomer { Id = Guid.NewGuid(), CustomerId = customerId });
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
}
