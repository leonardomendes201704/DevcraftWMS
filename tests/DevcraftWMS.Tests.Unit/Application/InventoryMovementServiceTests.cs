using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.InventoryMovements;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class InventoryMovementServiceTests
{
    [Fact]
    public async Task CreateMovement_Should_Return_Failure_When_OriginBalance_Is_Insufficient()
    {
        var customerContext = new FakeCustomerContext(Guid.NewGuid());
        var toLocationId = Guid.NewGuid();
        var originBalance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            LotId = null,
            QuantityOnHand = 2,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        };
        var balances = new List<InventoryBalance> { originBalance };

        var service = new InventoryMovementService(
            new FakeInventoryMovementRepository(),
            new FakeInventoryBalanceRepository(balances),
            new FakeLocationRepository(originBalance.LocationId, toLocationId),
            new FakeProductRepository(originBalance.ProductId),
            new FakeLotRepository(null),
            customerContext,
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(
            originBalance.LocationId,
            toLocationId,
            originBalance.ProductId,
            null,
            10,
            "Relocation",
            "REF-01",
            DateTime.UtcNow,
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("inventory.balance.insufficient");
    }

    [Fact]
    public async Task CreateMovement_Should_Decrease_Origin_And_Create_Destination_Balance()
    {
        var customerContext = new FakeCustomerContext(Guid.NewGuid());
        var fromLocationId = Guid.NewGuid();
        var toLocationId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var lotId = Guid.NewGuid();

        var originBalance = new InventoryBalance
        {
            Id = Guid.NewGuid(),
            LocationId = fromLocationId,
            ProductId = productId,
            LotId = lotId,
            QuantityOnHand = 12,
            QuantityReserved = 0,
            Status = InventoryBalanceStatus.Available
        };
        var balances = new List<InventoryBalance> { originBalance };
        var movementRepo = new FakeInventoryMovementRepository();
        var balanceRepo = new FakeInventoryBalanceRepository(balances);

        var service = new InventoryMovementService(
            movementRepo,
            balanceRepo,
            new FakeLocationRepository(fromLocationId, toLocationId),
            new FakeProductRepository(productId),
            new FakeLotRepository(new Lot { Id = lotId, ProductId = productId }),
            customerContext,
            new FakeDateTimeProvider());

        var result = await service.CreateAsync(
            fromLocationId,
            toLocationId,
            productId,
            lotId,
            5,
            "Relocation",
            "REF-02",
            DateTime.UtcNow,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        originBalance.QuantityOnHand.Should().Be(7);
        var destinationBalance = balances.SingleOrDefault(b => b.LocationId == toLocationId && b.ProductId == productId && b.LotId == lotId);
        destinationBalance.Should().NotBeNull();
        destinationBalance!.QuantityOnHand.Should().Be(5);
    }

    private sealed class FakeInventoryMovementRepository : IInventoryMovementRepository
    {
        public Task AddAsync(InventoryMovement movement, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(InventoryMovement movement, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryMovement?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryMovement?>(null);
        public Task<InventoryMovement?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<InventoryMovement?>(null);
        public Task<int> CountAsync(Guid? productId, Guid? fromLocationId, Guid? toLocationId, Guid? lotId, InventoryMovementStatus? status, DateTime? performedFromUtc, DateTime? performedToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryMovement>> ListAsync(Guid? productId, Guid? fromLocationId, Guid? toLocationId, Guid? lotId, InventoryMovementStatus? status, DateTime? performedFromUtc, DateTime? performedToUtc, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InventoryMovement>>(Array.Empty<InventoryMovement>());
        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default) => await action(cancellationToken);
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        private readonly List<InventoryBalance> _balances;

        public FakeInventoryBalanceRepository(List<InventoryBalance> balances)
        {
            _balances = balances;
        }

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult(_balances.Any(b => b.LocationId == locationId && b.ProductId == productId && b.LotId == lotId));
        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(_balances.Any(b => b.LocationId == locationId && b.ProductId == productId && b.LotId == lotId && b.Id != excludeId));
        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
        {
            _balances.Add(balance);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_balances.SingleOrDefault(b => b.Id == id));
        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_balances.SingleOrDefault(b => b.Id == id));
        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default) => Task.FromResult(_balances.SingleOrDefault(b => b.LocationId == locationId && b.ProductId == productId && b.LotId == lotId));
        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly Guid _fromLocationId;
        private readonly Guid _toLocationId;

        public FakeLocationRepository(Guid fromLocationId, Guid toLocationId)
        {
            _fromLocationId = fromLocationId;
            _toLocationId = toLocationId;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == _fromLocationId || id == _toLocationId)
            {
                return Task.FromResult<Location?>(new Location { Id = id, Code = "LOC" });
            }

            return Task.FromResult<Location?>(null);
        }
        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => GetByIdAsync(id, cancellationToken);
        public Task<int> CountAsync(Guid structureId, Guid? zoneId, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Location>> ListAsync(Guid structureId, Guid? zoneId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Guid _productId;

        public FakeProductRepository(Guid productId)
        {
            _productId = productId;
        }

        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(id == _productId ? new Product { Id = id, Code = "SKU", Name = "Product", BaseUomId = Guid.NewGuid() } : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => GetByIdAsync(id, cancellationToken);
        public Task<int> CountAsync(string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeLotRepository : ILotRepository
    {
        private readonly Lot? _lot;

        public FakeLotRepository(Lot? lot)
        {
            _lot = lot;
        }

        public Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_lot?.Id == id ? _lot : null);
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult(_lot?.Id == id ? _lot : null);
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountExpiringAsync(DateOnly expirationFrom, DateOnly expirationTo, LotStatus? status, CancellationToken cancellationToken = default) => Task.FromResult(0);
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow { get; } = new(2026, 2, 2, 10, 0, 0, DateTimeKind.Utc);
    }
}
