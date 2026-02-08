using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.InventoryCounts;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class InventoryCountServiceTests
{
    [Fact]
    public async Task Create_Should_Create_Items_From_Balances()
    {
        var customerId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Main" };
        var location = new Location { Id = Guid.NewGuid(), Code = "LOC-01" };
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-01", Name = "Product", BaseUomId = Guid.NewGuid() };

        var balances = new List<InventoryBalance>
        {
            new() { Id = Guid.NewGuid(), LocationId = location.Id, ProductId = product.Id, QuantityOnHand = 10, Product = product, Location = location },
            new() { Id = Guid.NewGuid(), LocationId = location.Id, ProductId = product.Id, QuantityOnHand = 5, Product = product, Location = location }
        };

        var countRepository = new FakeInventoryCountRepository();
        var service = new InventoryCountService(
            countRepository,
            new FakeWarehouseRepository(warehouse),
            new FakeLocationRepository(location),
            new FakeInventoryBalanceRepository(balances),
            new FakeInventoryMovementRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService(Guid.NewGuid()));

        var result = await service.CreateAsync(warehouse.Id, location.Id, null, null, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Complete_Should_Adjust_Balance_When_Divergence()
    {
        var customerId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Main" };
        var location = new Location { Id = Guid.NewGuid(), Code = "LOC-01" };
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-02", Name = "Product", BaseUomId = Guid.NewGuid() };

        var count = new InventoryCount
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = warehouse.Id,
            LocationId = location.Id,
            Status = InventoryCountStatus.InProgress,
            Items = new List<InventoryCountItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    InventoryCountId = Guid.NewGuid(),
                    LocationId = location.Id,
                    ProductId = product.Id,
                    UomId = product.BaseUomId,
                    QuantityExpected = 5,
                    QuantityCounted = 0,
                    Product = product,
                    Location = location
                }
            }
        };

        var balance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = location.Id,
            ProductId = product.Id,
            QuantityOnHand = 5
        };

        var balanceRepository = new FakeInventoryBalanceRepository(new List<InventoryBalance> { balance });
        var countRepository = new FakeInventoryCountRepository(count);
        var movementRepository = new FakeInventoryMovementRepository();

        var service = new InventoryCountService(
            countRepository,
            new FakeWarehouseRepository(warehouse),
            new FakeLocationRepository(location),
            balanceRepository,
            movementRepository,
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService(Guid.NewGuid()));

        var result = await service.CompleteAsync(count.Id, new List<CompleteInventoryCountItemInput>
        {
            new(count.Items.First().Id, 3)
        }, "ok", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        balanceRepository.Stored[0].QuantityOnHand.Should().Be(3);
        movementRepository.Stored.Should().HaveCount(1);
    }

    private sealed class FakeInventoryCountRepository : IInventoryCountRepository
    {
        public List<InventoryCount> Stored { get; } = new();

        public FakeInventoryCountRepository(InventoryCount? count = null)
        {
            if (count is not null)
            {
                Stored.Add(count);
            }
        }

        public Task AddAsync(InventoryCount count, CancellationToken cancellationToken = default)
        {
            Stored.Add(count);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(InventoryCountItem item, CancellationToken cancellationToken = default)
        {
            var count = Stored.FirstOrDefault(c => c.Id == item.InventoryCountId);
            count?.Items.Add(item);
            return Task.CompletedTask;
        }

        public Task<InventoryCount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<InventoryCount?>(Stored.SingleOrDefault(c => c.Id == id));

        public Task<InventoryCount?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<InventoryCount?>(Stored.SingleOrDefault(c => c.Id == id));

        public Task UpdateAsync(InventoryCount count, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<int> CountAsync(Guid? warehouseId, Guid? locationId, InventoryCountStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<InventoryCount>> ListAsync(Guid? warehouseId, Guid? locationId, InventoryCountStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryCount>>(Stored);
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse = null)
        {
            _warehouse = warehouse;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<string?> GetLatestCodeAsync(string prefix, CancellationToken cancellationToken = default) => Task.FromResult<string?>(null);
        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Warehouse?>(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Warehouse?>(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(string? code, string? name, WarehouseType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, WarehouseType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
        public Task<int> CountAsync(string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? search, string? code, string? name, WarehouseType? warehouseType, string? city, string? state, string? country, string? externalId, string? erpCode, string? costCenterCode, bool? isPrimary, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly Location? _location;

        public FakeLocationRepository(Location? location = null)
        {
            _location = location;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Location?>(_location?.Id == id ? _location : null);
        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Location?>(_location?.Id == id ? _location : null);
        public Task<int> CountAsync(Guid structureId, Guid? zoneId, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Location>> ListAsync(Guid structureId, Guid? zoneId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
        public Task<IReadOnlyList<Location>> ListByStructureAsync(Guid structureId, Guid? zoneId, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        public List<InventoryBalance> Stored { get; }

        public FakeInventoryBalanceRepository(List<InventoryBalance>? seed = null)
        {
            Stored = seed ?? new List<InventoryBalance>();
        }

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
        {
            Stored.Add(balance);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(Stored.SingleOrDefault(b => b.Id == id));
        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryBalance?>(Stored.SingleOrDefault(b => b.Id == id));
        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
            => Task.FromResult<InventoryBalance?>(Stored.SingleOrDefault(b => b.LocationId == locationId && b.ProductId == productId && b.LotId == lotId));
        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Stored);
        public Task<IReadOnlyList<InventoryBalance>> ListByLotAsync(Guid lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListAvailableForReservationAsync(Guid productId, Guid? lotId, ZoneType? zoneType, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListByProductAndZonesAsync(Guid productId, IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<IReadOnlyList<InventoryBalance>> ListByZonesAsync(IReadOnlyList<ZoneType> zoneTypes, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
    }

    private sealed class FakeInventoryMovementRepository : IInventoryMovementRepository
    {
        public List<InventoryMovement> Stored { get; } = new();

        public Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default)
        {
            Stored.Add(movement);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(InventoryMovement movement, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryMovement?>(null);
        public Task<InventoryMovement?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryMovement?>(null);
        public Task<int> CountAsync(Guid? productId, Guid? fromLocationId, Guid? toLocationId, Guid? lotId, InventoryMovementStatus? status, DateTime? performedFromUtc, DateTime? performedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryMovement>> ListAsync(Guid? productId, Guid? fromLocationId, Guid? toLocationId, Guid? lotId, InventoryMovementStatus? status, DateTime? performedFromUtc, DateTime? performedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryMovement>>(Array.Empty<InventoryMovement>());
        public Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default) => action(cancellationToken);
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public FakeCurrentUserService(Guid? userId)
        {
            UserId = userId;
        }

        public Guid? UserId { get; }
        public string? Email => null;
    }
}


