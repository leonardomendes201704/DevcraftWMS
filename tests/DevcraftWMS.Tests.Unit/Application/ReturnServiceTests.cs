using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Auth;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.Returns;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class ReturnServiceTests
{
    [Fact]
    public async Task Create_Should_Fail_When_Lot_Required_And_Missing()
    {
        var customerId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-RET", Name = "Return Product", TrackingMode = TrackingMode.Lot };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };

        var service = new ReturnService(
            new FakeReturnOrderRepository(),
            new FakeWarehouseRepository(new Warehouse { Id = warehouseId, Name = "Main" }),
            new FakeOutboundOrderRepository(),
            new FakeProductRepository(product),
            new FakeUomRepository(uom),
            new FakeLotRepository(),
            new FakeLocationRepository(),
            new FakeInventoryBalanceRepository(),
            new FakeInventoryMovementRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService(Guid.NewGuid()));

        var result = await service.CreateAsync(
            warehouseId,
            "RET-001",
            null,
            null,
            new List<CreateReturnItemInput>
            {
                new(product.Id, uom.Id, null, null, 2)
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("returns.item.tracking_required");
    }

    [Fact]
    public async Task Complete_Should_Update_Balance_When_Restock()
    {
        var customerId = Guid.NewGuid();
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Main" };
        var location = new Location { Id = Guid.NewGuid(), Code = "LOC-01", AllowLotTracking = true, AllowExpiryTracking = true };
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-RET2", Name = "Return Product", TrackingMode = TrackingMode.None };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "EA" };
        var order = new ReturnOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = warehouse.Id,
            ReturnNumber = "RET-002",
            Status = ReturnStatus.InProgress,
            Items = new List<ReturnItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ReturnOrderId = Guid.NewGuid(),
                    ProductId = product.Id,
                    UomId = uom.Id,
                    QuantityExpected = 5,
                    QuantityReceived = 0,
                    Product = product,
                    Uom = uom
                }
            }
        };

        var balanceRepository = new FakeInventoryBalanceRepository();

        var service = new ReturnService(
            new FakeReturnOrderRepository(order),
            new FakeWarehouseRepository(warehouse),
            new FakeOutboundOrderRepository(),
            new FakeProductRepository(product),
            new FakeUomRepository(uom),
            new FakeLotRepository(),
            new FakeLocationRepository(location),
            balanceRepository,
            new FakeInventoryMovementRepository(),
            new FakeCustomerContext(customerId),
            new FakeDateTimeProvider(DateTime.UtcNow),
            new FakeCurrentUserService(Guid.NewGuid()));

        var input = new List<CompleteReturnItemInput>
        {
            new(order.Items.First().Id, 5, ReturnItemDisposition.Restock, null, location.Id)
        };

        var result = await service.CompleteAsync(order.Id, input, "ok", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        balanceRepository.Stored.Should().HaveCount(1);
        balanceRepository.Stored[0].QuantityOnHand.Should().Be(5);
    }

    private sealed class FakeReturnOrderRepository : IReturnOrderRepository
    {
        private readonly ReturnOrder? _order;
        public List<ReturnOrder> Stored { get; } = new();

        public FakeReturnOrderRepository(ReturnOrder? order = null)
        {
            _order = order;
            if (order is not null)
            {
                Stored.Add(order);
            }
        }

        public Task AddAsync(ReturnOrder order, CancellationToken cancellationToken = default)
        {
            Stored.Add(order);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(ReturnItem item, CancellationToken cancellationToken = default)
        {
            var order = Stored.FirstOrDefault(o => o.Id == item.ReturnOrderId) ?? _order;
            order?.Items.Add(item);
            return Task.CompletedTask;
        }

        public Task<ReturnOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<ReturnOrder?>(Stored.SingleOrDefault(o => o.Id == id));

        public Task<ReturnOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<ReturnOrder?>(Stored.SingleOrDefault(o => o.Id == id) ?? _order);

        public Task UpdateAsync(ReturnOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task<int> CountAsync(Guid? warehouseId, string? returnNumber, ReturnStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(Stored.Count);

        public Task<IReadOnlyList<ReturnOrder>> ListAsync(Guid? warehouseId, string? returnNumber, ReturnStatus? status, bool? isActive, bool includeInactive, int pageNumber, int pageSize, string orderBy, string orderDir, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<ReturnOrder>>(Stored);

        public Task<bool> ReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
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

    private sealed class FakeOutboundOrderRepository : IOutboundOrderRepository
    {
        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<OutboundOrder?>(null);
        public Task<OutboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default) => Task.FromResult<OutboundOrder?>(null);
        public Task UpdateAsync(OutboundOrder order, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<int> CountAsync(Guid? warehouseId, string? orderNumber, OutboundOrderStatus? status, OutboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<OutboundOrder>> ListAsync(Guid? warehouseId, int pageNumber, int pageSize, string orderBy, string orderDir, string? orderNumber, OutboundOrderStatus? status, OutboundOrderPriority? priority, DateTime? createdFromUtc, DateTime? createdToUtc, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrder>>(Array.Empty<OutboundOrder>());
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        private readonly Product? _product;

        public FakeProductRepository(Product? product = null)
        {
            _product = product;
        }

        public Task AddAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Product product, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Product?>(_product?.Id == id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Product?>(_product?.Id == id ? _product : null);
        public Task<int> CountAsync(string? code, string? name, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, string? category, string? brand, string? ean, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Product>>(Array.Empty<Product>());
    }

    private sealed class FakeUomRepository : IUomRepository
    {
        private readonly Uom? _uom;

        public FakeUomRepository(Uom? uom = null)
        {
            _uom = uom;
        }

        public Task AddAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Uom uom, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Uom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Uom?>(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Uom?>(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(int pageNumber, int pageSize, string orderBy, string orderDir, string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
    }

    private sealed class FakeLotRepository : ILotRepository
    {
        private readonly List<Lot> _lots = new();

        public Task<bool> CodeExistsAsync(Guid productId, string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(Guid productId, string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task AddAsync(Lot lot, CancellationToken cancellationToken = default)
        {
            _lots.Add(lot);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Lot lot, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Lot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Lot?>(_lots.SingleOrDefault(l => l.Id == id));
        public Task<Lot?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult<Lot?>(_lots.SingleOrDefault(l => l.Id == id));
        public Task<Lot?> GetByCodeAsync(Guid productId, string code, CancellationToken cancellationToken = default)
            => Task.FromResult<Lot?>(_lots.SingleOrDefault(l => l.ProductId == productId && l.Code == code));
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountAsync(Guid productId, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Lot>> ListAsync(Guid productId, int pageNumber, int pageSize, string orderBy, string orderDir, string? code, LotStatus? status, DateOnly? expirationFrom, DateOnly? expirationTo, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Lot>>(Array.Empty<Lot>());
        public Task<int> CountExpiringAsync(DateOnly expirationFrom, DateOnly expirationTo, LotStatus? status, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
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
        public List<InventoryBalance> Stored { get; } = new();

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
            => Task.FromResult<IReadOnlyList<InventoryBalance>>(Array.Empty<InventoryBalance>());
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
