using FluentAssertions;
using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.InventoryBalances;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class InventoryBalanceServiceTests
{
    [Fact]
    public async Task Create_Should_Return_Failure_When_Customer_Context_Missing()
    {
        var service = BuildService(customerId: null, allowNullCustomer: true);

        var result = await service.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), null, 10, 0, InventoryBalanceStatus.Available, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("customers.context.required");
    }

    [Fact]
    public async Task Create_Should_Return_Failure_When_Reserved_Exceeds_OnHand()
    {
        var service = BuildService();

        var result = await service.CreateAsync(Guid.NewGuid(), Guid.NewGuid(), null, 5, 10, InventoryBalanceStatus.Available, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("inventory.balance.invalid_quantities");
    }

    [Fact]
    public async Task Create_Should_Return_Success_When_Valid()
    {
        var locationId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var balanceRepository = new FakeInventoryBalanceRepository();
        var service = BuildService(balanceRepository, locationId, productId);

        var result = await service.CreateAsync(locationId, productId, null, 12, 2, InventoryBalanceStatus.Available, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        balanceRepository.Added.Should().NotBeNull();
    }

    private static InventoryBalanceService BuildService(
        FakeInventoryBalanceRepository? balanceRepository = null,
        Guid? locationId = null,
        Guid? productId = null,
        Guid? customerId = null,
        bool allowNullCustomer = false)
    {
        var locationRepo = new FakeLocationRepository(locationId);
        var productRepo = new FakeProductRepository(productId);
        var lotRepo = new FakeLotRepository(null);
        var resolvedCustomerId = allowNullCustomer ? customerId : customerId ?? Guid.NewGuid();
        var customerContext = new FakeCustomerContext(resolvedCustomerId);

        return new InventoryBalanceService(
            balanceRepository ?? new FakeInventoryBalanceRepository(),
            locationRepo,
            productRepo,
            lotRepo,
            customerContext);
    }

    private sealed class FakeInventoryBalanceRepository : IInventoryBalanceRepository
    {
        public InventoryBalance? Added { get; private set; }

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task<bool> ExistsAsync(Guid locationId, Guid productId, Guid? lotId, Guid excludeId, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task AddAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
        {
            Added = balance;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(InventoryBalance balance, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<InventoryBalance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<InventoryBalance?>(null);

        public Task<InventoryBalance?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<InventoryBalance?>(null);

        public Task<InventoryBalance?> GetTrackedByKeyAsync(Guid locationId, Guid productId, Guid? lotId, CancellationToken cancellationToken = default)
            => Task.FromResult<InventoryBalance?>(null);

        public Task<int> CountAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<IReadOnlyList<InventoryBalance>> ListAsync(Guid? locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());

        public Task<int> CountByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);

        public Task<IReadOnlyList<InventoryBalance>> ListByLocationAsync(Guid locationId, Guid? productId, Guid? lotId, InventoryBalanceStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
    }

    private sealed class FakeLocationRepository : ILocationRepository
    {
        private readonly Guid? _locationId;

        public FakeLocationRepository(Guid? locationId)
        {
            _locationId = locationId;
        }

        public Task<bool> CodeExistsAsync(Guid structureId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid structureId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Location location, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_locationId.HasValue && _locationId.Value == id ? new Location { Id = id } : null);
        public Task<Location?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Location?>(null);
        public Task<int> CountAsync(Guid structureId, Guid? zoneId, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Location>> ListAsync(Guid structureId, Guid? zoneId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? barcode, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Location>>(Array.Empty<Location>());
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Guid? _productId;

        public FakeProductRepository(Guid? productId)
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
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_productId.HasValue && _productId.Value == id ? new Product { Id = id, BaseUomId = Guid.NewGuid(), Code = "SKU", Name = "Test" } : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Product?>(null);
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
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<Lot?>(null);
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<int> CountExpiringAsync(DateOnly fromDate, DateOnly toDate, LotStatus? status, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
    }

    private sealed class FakeCustomerContext : ICustomerContext
    {
        public FakeCustomerContext(Guid? customerId)
        {
            CustomerId = customerId;
        }

        public Guid? CustomerId { get; }
    }
}
