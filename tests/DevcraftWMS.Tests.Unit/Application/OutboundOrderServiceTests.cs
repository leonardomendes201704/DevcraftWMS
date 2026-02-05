using DevcraftWMS.Application.Abstractions;
using DevcraftWMS.Application.Abstractions.Customers;
using DevcraftWMS.Application.Features.OutboundOrders;
using DevcraftWMS.Domain.Entities;
using DevcraftWMS.Domain.Enums;
using FluentAssertions;

namespace DevcraftWMS.Tests.Unit.Application;

public sealed class OutboundOrderServiceTests
{
    [Fact]
    public async Task Create_Should_Return_Failure_When_Customer_Context_Missing()
    {
        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(),
            new FakeWarehouseRepository(),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeCustomerContext(null));

        var result = await service.CreateAsync(
            Guid.NewGuid(),
            "OS-1000",
            null,
            null,
            DateOnly.FromDateTime(DateTime.UtcNow),
            null,
            new List<CreateOutboundOrderItemInput>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), 1, null, null)
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("customers.context.required");
    }

    [Fact]
    public async Task Create_Should_Create_Order_With_Items()
    {
        var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "WH-OUT" };
        var product = new Product { Id = Guid.NewGuid(), Code = "SKU-OUT", Name = "Outbound", TrackingMode = TrackingMode.None };
        var uom = new Uom { Id = Guid.NewGuid(), Code = "UN" };

        var service = new OutboundOrderService(
            new FakeOutboundOrderRepository(),
            new FakeWarehouseRepository(warehouse),
            new FakeProductRepository(product),
            new FakeUomRepository(uom),
            new FakeCustomerContext(Guid.NewGuid()));

        var result = await service.CreateAsync(
            warehouse.Id,
            "OS-2000",
            "REF-2000",
            "Carrier",
            DateOnly.FromDateTime(DateTime.UtcNow),
            "Notes",
            new List<CreateOutboundOrderItemInput>
            {
                new(product.Id, uom.Id, 5, null, null)
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(1);
        result.Value.OrderNumber.Should().Be("OS-2000");
    }

    [Fact]
    public async Task Release_Should_Set_Status_And_Parameters()
    {
        var customerId = Guid.NewGuid();
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-RELEASE-1",
            Status = OutboundOrderStatus.Registered,
            Priority = OutboundOrderPriority.Normal
        };

        var repository = new FakeOutboundOrderRepository(order);
        var service = new OutboundOrderService(
            repository,
            new FakeWarehouseRepository(),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeCustomerContext(customerId));

        var result = await service.ReleaseAsync(
            order.Id,
            OutboundOrderPriority.High,
            OutboundOrderPickingMethod.Batch,
            DateTime.UtcNow.AddHours(1),
            DateTime.UtcNow.AddHours(3),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(OutboundOrderStatus.Released);
        result.Value.Priority.Should().Be(OutboundOrderPriority.High);
        result.Value.PickingMethod.Should().Be(OutboundOrderPickingMethod.Batch);
    }

    [Fact]
    public async Task Release_Should_Fail_When_Window_Invalid()
    {
        var customerId = Guid.NewGuid();
        var order = new OutboundOrder
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            WarehouseId = Guid.NewGuid(),
            OrderNumber = "OS-RELEASE-2",
            Status = OutboundOrderStatus.Registered
        };

        var repository = new FakeOutboundOrderRepository(order);
        var service = new OutboundOrderService(
            repository,
            new FakeWarehouseRepository(),
            new FakeProductRepository(),
            new FakeUomRepository(),
            new FakeCustomerContext(customerId));

        var result = await service.ReleaseAsync(
            order.Id,
            OutboundOrderPriority.Normal,
            OutboundOrderPickingMethod.SingleOrder,
            DateTime.UtcNow.AddHours(2),
            DateTime.UtcNow.AddHours(1),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("outbound_orders.order.window_invalid");
    }

    private sealed class FakeOutboundOrderRepository : IOutboundOrderRepository
    {
        private readonly List<OutboundOrder> _orders = new();

        public FakeOutboundOrderRepository()
        {
        }

        public FakeOutboundOrderRepository(OutboundOrder order)
        {
            _orders.Add(order);
        }

        public Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.Any(o => o.OrderNumber == orderNumber));

        public Task AddAsync(OutboundOrder order, CancellationToken cancellationToken = default)
        {
            _orders.Add(order);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(OutboundOrderItem item, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<OutboundOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.SingleOrDefault(o => o.Id == id));

        public Task<OutboundOrder?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_orders.SingleOrDefault(o => o.Id == id));

        public Task UpdateAsync(OutboundOrder order, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<int> CountAsync(
            Guid? warehouseId,
            string? orderNumber,
            OutboundOrderStatus? status,
            OutboundOrderPriority? priority,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);

        public Task<IReadOnlyList<OutboundOrder>> ListAsync(
            Guid? warehouseId,
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? orderNumber,
            OutboundOrderStatus? status,
            OutboundOrderPriority? priority,
            DateTime? createdFromUtc,
            DateTime? createdToUtc,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<OutboundOrder>>(Array.Empty<OutboundOrder>());
    }

    private sealed class FakeWarehouseRepository : IWarehouseRepository
    {
        private readonly Warehouse? _warehouse;

        public FakeWarehouseRepository(Warehouse? warehouse = null)
        {
            _warehouse = warehouse;
        }

        public Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task UpdateAsync(Warehouse warehouse, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<Warehouse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<Warehouse?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_warehouse?.Id == id ? _warehouse : null);
        public Task<int> CountAsync(
            string? search,
            string? code,
            string? name,
            WarehouseType? warehouseType,
            string? city,
            string? state,
            string? country,
            string? externalId,
            string? erpCode,
            string? costCenterCode,
            bool? isPrimary,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Warehouse>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? search,
            string? code,
            string? name,
            WarehouseType? warehouseType,
            string? city,
            string? state,
            string? country,
            string? externalId,
            string? erpCode,
            string? costCenterCode,
            bool? isPrimary,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Warehouse>>(Array.Empty<Warehouse>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
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
            => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_product?.Id == id ? _product : null);
        public Task<Product?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult<Product?>(null);
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> EanExistsAsync(string ean, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> ErpCodeExistsAsync(string erpCode, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<int> CountAsync(
            string? code,
            string? name,
            string? category,
            string? brand,
            string? ean,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default) => Task.FromResult(0);
        public Task<IReadOnlyList<Product>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            string? category,
            string? brand,
            string? ean,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
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
            => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<Uom?> GetTrackedByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_uom?.Id == id ? _uom : null);
        public Task<int> CountAsync(string? code, string? name, UomType? type, bool? isActive, bool includeInactive, CancellationToken cancellationToken = default)
            => Task.FromResult(0);
        public Task<IReadOnlyList<Uom>> ListAsync(
            int pageNumber,
            int pageSize,
            string orderBy,
            string orderDir,
            string? code,
            string? name,
            UomType? type,
            bool? isActive,
            bool includeInactive,
            CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<Uom>>(Array.Empty<Uom>());
        public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<bool> CodeExistsAsync(string code, Guid excludeId, CancellationToken cancellationToken = default) => Task.FromResult(false);
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
